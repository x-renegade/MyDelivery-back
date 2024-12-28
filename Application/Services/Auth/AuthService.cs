using Application.Common.Contracts;
using Application.Common.Exceptions;
using Application.Common.Models.User.Requests;
using Application.Common.Models.User.Responses;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Services.Auth;

public class AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IConfiguration configuration) : IAuthService
{

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = tokenService.ValidateToken(request.AccessToken) ??
            throw new InvalidTokenException("Invalid access token or refresh token");

        var user = await userManager.FindByNameAsync(principal.Identity?.Name!) ??
            throw new UserNotFoundException("User does not exist");

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new InvalidTokenException("Invalid refresh token or token expired");

        var (accessToken, refreshToken, expiration) = await GenerateTokensAsync(user, principal.Claims.ToList());

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = expiration
        };
    }

    public async Task RevokeAsync(User user)
    {
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await userManager.UpdateAsync(user);
        await signInManager.SignOutAsync();
    }

    public async Task RevokeAllAsync()
    {
        var users = userManager.Users.ToList();
        await Parallel.ForEachAsync(users, async (user, _) =>
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            await userManager.UpdateAsync(user);
        });
    }

    public async Task<SignInResponse> SignInAsync(SignInRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ??
            throw new UserNotFoundException($"User with email {request.Email} not found.");

        var result = await signInManager.PasswordSignInAsync(request.Email, request.Password, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                throw new UserException("User account is locked.");
            throw new PasswordIncorrectException("The password is incorrect.");
        }

        // Получаем роли пользователя
        var userRoles = await userManager.GetRolesAsync(user);

        // Преобразуем список ролей в список Claims
        var claims = userRoles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        // Генерируем токены
        (string accessToken, string refreshToken, DateTime expiration) = await GenerateTokensAsync(user, claims);

        return new SignInResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = expiration
        };
    }



    public async Task<bool> SignUpAsync(SignUpRequest request)
    {
        // Проверяем, существует ли уже пользователь с таким email
        var isEmailExist = await userManager.FindByEmailAsync(request.Email);
        if (isEmailExist != null)
        {
            // Если email уже существует, выбрасываем исключение
            throw new UserAlreadyExistsException($"User with email {request.Email} already exists.");
        }

        // Создаем нового пользователя
        User user = new()
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Email,
            FirstName = request.FirstName
        };

        var result = await userManager.CreateAsync(user, request.Password);

        // Проверка успешности создания пользователя
        if (!result.Succeeded)
        {
            throw new UserException("Failed to create user.");
        }

        // Добавляем пользователя в роль User
        await userManager.AddToRoleAsync(user, UserRoles.User);

        return true;
    }

    public async Task<(string AccessToken, string RefreshToken, DateTime Expiration)> GenerateTokensAsync(User user, IList<Claim> additionalClaims)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Добавляем дополнительные claims (например, роли)
        claims.AddRange(additionalClaims);

        var token = tokenService.GenerateToken(claims);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(
            int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out var days) ? days : 7
        );

        await userManager.UpdateAsync(user);

        return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken, token.ValidTo);
    }

}


