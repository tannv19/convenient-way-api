using Microsoft.IdentityModel.Tokens;
using ship_convenient.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ship_convenient.Helper
{
    public class JWTHelper
    {
        public static string GenerateJWTToken(Account account, string jwtKey) {
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(jwtKey);
            SecurityTokenDescriptor tokenDescription = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, account.UserName),
                    new Claim("id", account.Id.ToString()),
                    new Claim("role", account.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes),
                SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescription);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
