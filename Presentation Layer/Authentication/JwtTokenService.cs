using Business_Layer.Business;
using Microsoft.IdentityModel.Tokens;
using Models.DTO;
using Presentation_Layer.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.Authentication
{
    public class JwtTokenService
    {
        public async Task<string> CreateToken(User user)
        {
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            List<RoleDTO> roles = await user.GetRoles();

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(JwtOptions.Expires >= 0 ? JwtOptions.Expires : 30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
