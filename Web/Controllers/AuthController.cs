using Application.Common.Contracts.Services;
using Application.Common.Models.User.Requests.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        _ = await authService.SignUpAsync(request);
        return Ok(new { message = "User created successfully" });
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var response = await authService.SignInAsync(request);

        return Ok(new { accessToken = response.AccessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var response = await authService.RefreshTokenAsync();
        return Ok(response); 
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await authService.SignOutAsync();
        return Ok(new { message = "User signed out successfully" });
    }
}