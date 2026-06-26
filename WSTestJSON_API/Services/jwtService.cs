using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WSTestJSON_API.Models;

namespace WSTestJSON_API.Services
{
    public class jwtService : IjwtService
    {
        private readonly IConfiguration _configuration;

        public jwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerarToken(Usuarios usuario)
        {
            //generar jwt
            var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var sign = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario?.IdUser.ToString()),
                new Claim(ClaimTypes.UserData, usuario.Email),
                new Claim(ClaimTypes.Role, usuario?.IdRol.ToString())
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims: claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: sign);
            return new JwtSecurityTokenHandler().WriteToken(token);
            
        }
    }
}
