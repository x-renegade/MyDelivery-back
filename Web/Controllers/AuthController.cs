using Application.Common.Contracts;
using Application.Common.Models.User.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
    IAuthService authService) : ControllerBase
    {
        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignIn(SignInRequest request)
            => Ok(await authService.SignIn(request));

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUp(SignUpRequest request)
            => Ok(await authService.SignUp(request));
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
            => Ok(await authService.RefreshToken(request));
    }
}
