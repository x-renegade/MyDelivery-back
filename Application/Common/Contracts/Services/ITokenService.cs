using Application.Common.Models.User.Jwt;
using Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Common.Contracts.Services;

public interface ITokenService
{
    public JwtSecurityToken GenerateAccessToken(List<Claim> authClaims);
    public ClaimsPrincipal ValidateToken(string token);
    public string GenerateRefreshToken();
    public TokenData GetData(string accessToken);
    public Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user, IList<Claim> additionalClaims);
}
