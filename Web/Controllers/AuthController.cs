using Application.Common.Contracts.Services;
using Application.Common.Exceptions;
using Application.Common.Models.User.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    // Регистрация пользователя
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        _ = await authService.SignUpAsync(request);
        return Ok(new { message = "User created successfully" });
    }

    // Вход пользователя
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var response = await authService.SignInAsync(request);

        return Ok(new { accessToken = response.AccessToken });
    }

    // Обновление токена (Refresh Token)
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await authService.RefreshTokenAsync(request);
        return Ok(response); // Вернем новый токен
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await authService.SignOutAsync();
        return Ok(new { message = "User signed out successfully" });
    }
}