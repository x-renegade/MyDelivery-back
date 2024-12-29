using Application.Common.Contracts;
using Application.Common.Contracts.Repositories;
using Application.Common.Contracts.Services;
using Application.Common.Exceptions;
using Application.Common.Models.User.Requests;
using Application.Common.Models.User.Responses;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Services.Auth;

public class AuthService(IUserRepository userRepository, ITokenService tokenService, IAppConfiguration appConfiguration, IHttpContextAccessor httpContextAccessor) : IAuthService
{
    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var context = httpContextAccessor.HttpContext ??
            throw new InvalidOperationException("No active HTTP context.");

        var accessToken = request.AccessToken;

        var refreshToken = context.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            throw new InvalidTokenException("Refresh Token is missing.");

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = tokenHandler.ReadJwtToken(accessToken);
        }
        catch (Exception)
        {
            throw new InvalidTokenException("Access Token is invalid.");
        }

        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
        if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
            throw new InvalidTokenException("Access Token does not contain a valid email claim.");
        var email = emailClaim.Value;

        var user = await userRepository.GetUserByEmailAsync(email) ??
            throw new UserNotFoundException("User does not exist.");
        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new InvalidTokenException("Invalid or expired Refresh Token.");

        var userRoles = await userRepository.GetUserRolesAsync(user);
        var claims = userRoles.Select(role => new Claim("role", role)).ToList();
        var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(user, claims);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(appConfiguration.GetValue("JWT:RefreshTokenValidityInDays")))
        };
        context.Response.Cookies.Append("refreshToken", newRefreshToken, cookieOptions);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken
        };
    }

    public async Task RevokeAsync(User user)
    {
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await userRepository.UpdateUserAsync(user);
    }

    public async Task RevokeAllAsync()
    {
        var users = await userRepository.GetAllUsersAsync();
        await Parallel.ForEachAsync(users, async (user, _) =>
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            await userRepository.UpdateUserAsync(user);
        });
    }

    public async Task<SignInResponse> SignInAsync(SignInRequest request)
    {
        // Проверяем, существует ли пользователь с указанным email
        var user = await userRepository.GetUserByEmailAsync(request.Email) ??
            throw new UserNotFoundException("User not found.");

        // Проверяем пароль пользователя
        var isPasswordValid = await userRepository.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            throw new PasswordIncorrectException("The password is incorrect.");

        await userRepository.SignInUserAsync(user, isPersistent: false);

        // Получаем роли пользователя
        var userRoles = await userRepository.GetUserRolesAsync(user);

        // Генерируем токены
        var claims = userRoles.Select(role => new Claim("role", role)).ToList();
        (string accessToken, string refreshToken) = await GenerateTokensAsync(user, claims);

        var context = httpContextAccessor.HttpContext; 
        if (context is not null)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(appConfiguration.GetValue("JWT:RefreshTokenValidityInDays")))
            };

            context.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        // Возвращаем только accessToken
        return new SignInResponse
        {
            AccessToken = accessToken
        };
    }

    public async Task<bool> SignUpAsync(SignUpRequest request)
    {
        var isEmailExist = await userRepository.GetUserByEmailAsync(request.Email);
        if (isEmailExist != null)
            throw new UserAlreadyExistsException($"User with email {request.Email} already exists.");

        User user = new()
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Email,
            FirstName = request.FirstName
        };

        var result = await userRepository.CreateUserAsync(user, request.Password);
        if (!result)
            throw new UserException("Failed to create user.");

        await userRepository.AddUserToRoleAsync(user, UserRoles.User);

        return true;
    }
    public async Task SignOutAsync()
    {
        var context = httpContextAccessor.HttpContext ??
            throw new InvalidOperationException("No active HTTP context.");

        // Вызываем SignOutUserAsync для завершения сессии
        await userRepository.SignOutUserAsync();

        // Удаляем RefreshToken из куки
        context.Response.Cookies.Delete("refreshToken");
    }


    public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user, IList<Claim> additionalClaims)
    {
        var claims = new List<Claim>
    {
        new("email", user.Email!),
        new ("firstName",user.FirstName),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

        claims.AddRange(additionalClaims);

        var token = tokenService.GenerateToken(claims);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(
            int.TryParse(appConfiguration.GetValue("JWT:RefreshTokenValidityInDays"), out var days) ? days : 7
        );

        await userRepository.UpdateUserAsync(user);

        return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken);
    }

}
