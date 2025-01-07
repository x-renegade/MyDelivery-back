
using Application.Common.Contracts;
using Application.Common.Contracts.Services;
using Application.Common.Exceptions;
using Microsoft.Extensions.Primitives;

namespace Api.Services;

public class TokenExtractionService(IHttpContextAccessor httpContextAccessor, IAppConfiguration appConfiguration) : ITokenExtractionService
{
    public string GetAccessFromHeader()
    {
        var context = httpContextAccessor.HttpContext ??
           throw new InvalidOperationException("No active HTTP context.");

        if (!context.Request.Headers.TryGetValue("Authorization", out StringValues authorizationHeader))
            throw new InvalidOperationException("Authorization header is missing.");
        var access = authorizationHeader.ToString().Replace("Bearer ", "").Trim();
        return access;
    }

    public string GetRefreshFromCookie()
    {
        var context = httpContextAccessor.HttpContext ??
          throw new InvalidOperationException("No active HTTP context.");

        var refreshToken = context.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            throw new InvalidTokenException("Refresh Token is missing.");
        return refreshToken;
    }

    public void RemoveRefreshFromCookie()
    {
        var context = httpContextAccessor.HttpContext ??
        throw new InvalidOperationException("No active HTTP context.");

        context.Response.Cookies.Delete("refreshToken");
    }

    public void SetRefreshToCookie(string refreshToken)
    {
        var context = httpContextAccessor.HttpContext ??
         throw new InvalidOperationException("No active HTTP context.");
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(appConfiguration.GetValue("JWT:RefreshTokenValidityInDays")))
        };
        context.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
