

using Application.Common.Contracts;
using Application.Common.Exceptions;
using Application.Common.Models.User.Requests;
using Application.Common.Models.User.Responses;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Services
{
    public class AuthService(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    ITokenService tokenService) : IAuthService
    {
        public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request)
        {
            var principal = tokenService.ValidateToken(request.AccessToken) ??
                throw UserException.BadRequestException("Invalid access token or refresh token");

            var user = await userManager.FindByNameAsync(principal.Identity!.Name!) ??
                 throw UserException.BadRequestException(UserErrorMessage.UserNotExist);

            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw UserException.BadRequestException("Invalid access token or refresh token");

            var newAccessToken = tokenService.GenerateToken(principal.Claims.ToList());
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await userManager.UpdateAsync(user);
            return new RefreshTokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken,
                Expiration = newAccessToken.ValidTo
            };
        }

        public Task Revoke(User user)
        {
            throw new NotImplementedException();
        }

        public Task RevokeAll()
        {
            throw new NotImplementedException();
        }

        public async Task<SignInResponse> SignIn(SignInRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName) ??
                throw UserException.BadRequestException(UserErrorMessage.UserNotExist);

            if (!await userManager.CheckPasswordAsync(user, request.Password))
                throw UserException.BadRequestException(UserErrorMessage.PasswordIncorrect);


            var userRoles = await userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = tokenService.GenerateToken(authClaims);
            var refreshToken = tokenService.GenerateRefreshToken();

            _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

            await userManager.UpdateAsync(user);
            return new SignInResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            };
        }

        public async Task<bool> SignUp(SignUpRequest request)
        {
            var isUserNameExist = await userManager.FindByNameAsync(request.UserName);
            if (isUserNameExist is not null)
                throw UserException.UserAlreadyExistsException(request.UserName);

            var isEmailExist = await userManager.FindByEmailAsync(request.Email);
            if (isEmailExist is not null)
                throw UserException.UserAlreadyExistsException(request.Email);

            User user = new()
            {
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.UserName
            };
            var result = await userManager.CreateAsync(user, request.Password);

            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            await userManager.AddToRoleAsync(user, UserRoles.User);
            return result.Succeeded;
        }
    }
}
