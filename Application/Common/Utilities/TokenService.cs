using Application.Common.Contracts;
using Application.Common.Contracts.Services;
using Application.Common.Exceptions;
using Application.Common.Models.User.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Utilities
{
    public class TokenService(IAppConfiguration configuration) : ITokenService
    {
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public JwtSecurityToken GenerateAccessToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue("JWT:Secret")));
            _ = int.TryParse(configuration.GetValue("JWT:TokenValidityInMinutes"), out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: configuration.GetValue("JWT:ValidIssuer"),
                audience: configuration.GetValue("JWT:ValidAudience"),
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

        public ClaimsPrincipal ValidateToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue("JWT:Secret"))),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException($"Invalid {nameof(accessToken)}");

            return principal;
        }
        public TokenData GetData(string accessToken)
        {
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
            var data = new TokenData();

            var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");
            if (idClaim == null || string.IsNullOrEmpty(idClaim.Value))
                throw new InvalidTokenException("Access Token does not contain a valid id claim.");
            data.Id = idClaim.Value;

            var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "jti");
            if (jtiClaim == null || string.IsNullOrEmpty(jtiClaim.Value))
                throw new InvalidTokenException("Access Token does not contain a valid jti claim.");
            data.Jti = jtiClaim.Value;

            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
                throw new InvalidTokenException("Access Token does not contain a valid email claim.");
            data.Email = emailClaim.Value;

            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "firstName");
            if (nameClaim == null || string.IsNullOrEmpty(nameClaim.Value))
                throw new InvalidTokenException("Access Token does not contain a valid firstName claim.");
            data.Email = nameClaim.Value;

            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
            if (roleClaim == null || string.IsNullOrEmpty(roleClaim.Value))
                throw new InvalidTokenException("Access Token does not contain a valid role claim.");
            data.Email = roleClaim.Value;

            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
            if (expClaim == null || string.IsNullOrEmpty(expClaim.Value))
                throw new InvalidTokenException("Access Token does not contain a valid exp claim.");
            data.Email = expClaim.Value;

            return data;
        }

       
    }
}
