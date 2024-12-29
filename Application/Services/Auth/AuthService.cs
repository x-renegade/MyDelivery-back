﻿
using Application.Common.Contracts;
using Application.Common.Contracts.Repositories;
using Application.Common.Contracts.Services;
using Application.Common.Exceptions;
using Application.Common.Models.User.Requests;
using Application.Common.Models.User.Responses;
using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Services.Auth;

public class AuthService(IUserRepository userRepository, ITokenService tokenService, IAppConfiguration appConfiguration) : IAuthService
{
    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(request.AccessToken);

        // Извлекаем exp
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
        if (expClaim == null || !long.TryParse(expClaim.Value, out var expUnix))
            throw new InvalidTokenException("Token does not contain a valid expiration claim.");

        var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        if (DateTime.UtcNow > expirationDate)
            throw new InvalidTokenException("Token has expired.");


        var principal = tokenService.ValidateToken(request.AccessToken) ??
            throw new InvalidTokenException("Invalid access token or refresh token");

        var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? throw new UserNotFoundException("Email not found in the token");

        var user = await userRepository.GetUserByEmailAsync(email)
            ?? throw new UserNotFoundException("User does not exist");


        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new InvalidTokenException("Invalid refresh token or token expired");

        var (accessToken, refreshToken) = await GenerateTokensAsync(user, principal.Claims.ToList());

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
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
        var isPasswordValid = await userRepository.SignInAsync(request.Email, request.Password);
        if (!isPasswordValid)
        {
            throw new PasswordIncorrectException("The password is incorrect.");
        }

        // Получаем роли пользователя
        var userRoles = await userRepository.GetUserRolesAsync(user);

        // Генерируем токены
        var claims = userRoles.Select(role => new Claim("role", role)).ToList();
        (string accessToken, string refreshToken) = await GenerateTokensAsync(user, claims);

        return new SignInResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
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
