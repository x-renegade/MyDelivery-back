
using Application.Common.Contracts.Repositories;
using Application.Common.Contracts.Services;
using Application.Common.Exceptions;
using System.Data;
using System.Security.Claims;
using Shared.Identity;
using Application.Common.Models.User.Responses.Auth;
using Application.Common.Models.User.Requests.Auth;

namespace Infrastructure.Services.Auth;

public class AuthService(IUserRepository userRepository,
    ITokenService tokenService,ITokenExtractionService tokenExtractionService) : IAuthService
{
    public async Task<RefreshTokenResponse> RefreshTokenAsync()
    {


        var refreshToken = tokenExtractionService.GetRefreshFromCookie();

        var accessToken = tokenExtractionService.GetAccessFromHeader();

        var tokenData=tokenService.GetData(accessToken);

        var user = await userRepository.GetUserByIdAsync(tokenData.Id) ??
            throw new UserNotFoundException("User does not exist.");
        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new InvalidTokenException("Invalid or expired Refresh Token.");

        var userRoles = await userRepository.GetUserRolesAsync(user);
        var claims = userRoles.Select(role => new Claim("roles", role)).ToList();
        var (newAccessToken, newRefreshToken) = await tokenService.GenerateTokensAsync(user, claims);

        
        tokenExtractionService.SetRefreshToCookie(newRefreshToken);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken
        };
    }

    //public async Task RevokeAsync(User user)
    //{
    //    user.RefreshToken = null;
    //    user.RefreshTokenExpiryTime = DateTime.MinValue;
    //    await userRepository.UpdateUserAsync(user);
    //}

    //public async Task RevokeAllAsync()
    //{
    //    var users = await userRepository.GetAllUsersAsync();
    //    await Parallel.ForEachAsync(users, async (user, _) =>
    //    {
    //        user.RefreshToken = null;
    //        user.RefreshTokenExpiryTime = DateTime.MinValue;
    //        await userRepository.UpdateUserAsync(user);
    //    });
    //}

    public async Task<SignInResponse> SignInAsync(SignInRequest request)
    {
        // Проверяем, существует ли пользователь с указанным email
        var user = await userRepository.GetUserByEmailAsync(request.Email) ??
            throw new UserNotFoundException("User not found.");

        // Проверяем пароль пользователя
        var isPasswordValid = await userRepository.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            throw new PasswordIncorrectException("The password is incorrect.");

        // Получаем роли пользователя
        var userRoles = await userRepository.GetUserRolesAsync(user);

        // Генерируем токены
        var claims = userRoles.Select(role => new Claim("roles", role)).ToList();
        (string accessToken, string refreshToken) = await tokenService.GenerateTokensAsync(user, claims);

        tokenExtractionService.SetRefreshToCookie(refreshToken);

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

        return await userRepository.SignUpAsync(user,request.Password);
    }
    public async Task SignOutAsync()
    {

        var tokenData = tokenService.GetData(tokenExtractionService.GetAccessFromHeader());
        var user= await userRepository.GetUserByIdAsync(tokenData.Id);
        // Вызываем SignOutUserAsync для завершения сессии
        await userRepository.SignOutUserAsync(user!);

        // Удаляем RefreshToken из куки
        tokenExtractionService.RemoveRefreshFromCookie();
    }

}
