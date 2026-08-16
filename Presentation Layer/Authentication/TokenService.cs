using Business_Layer.Business;
using Microsoft.IdentityModel.Tokens;
using Models.DTO;
using Models.Requests;
using Models.Response;
using PayoutsSdk.Core;
using Presentation_Layer.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCryptHelper = BCrypt.Net.BCrypt;

namespace Presentation_Layer.Authentication
{
    public class TokenService
    {
        public async Task<string> CreateAccessToken(User user)
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


        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

}
