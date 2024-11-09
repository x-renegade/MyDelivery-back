
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Common.Contracts
{
    public interface ITokenService
    {
        public JwtSecurityToken GenerateToken(List<Claim> authClaims);
        public ClaimsPrincipal ValidateToken(string token);
        public string GenerateRefreshToken();
    }
}
