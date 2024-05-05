using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.Common
{
    public class JWTValidator
    {
        static public bool ValidateToken(string? authToken, string secretKey)
        {
            try
            {
                if(authToken is null) return false;
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();
                IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out SecurityToken validatedToken);
                // var readedToken = tokenHandler.ReadToken(authToken);
                var jwtSecurityToken = tokenHandler.ReadJwtToken(authToken);
                var UserId = jwtSecurityToken?.Claims?.FirstOrDefault(i => i.Type == "sub")?.Value;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string? GetClaim(ClaimsPrincipal user, string key) { 
            return user?.Claims?.FirstOrDefault(c => {
                var x = c;
                return false;
            })?.Value; 
        }

        static private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true, 
                ValidateAudience = false,
                ValidateIssuer = false,  
                // ValidIssuer = "http://localhost:5280",
                // ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("4f858f9c-ab7d-4db4-8c2d-d2aaa67d1b83")) // The same key as the one that generate the token
            };
        }
    }
}