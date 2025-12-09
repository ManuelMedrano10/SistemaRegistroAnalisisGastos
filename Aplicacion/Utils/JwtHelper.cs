using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Aplicacion.Utils
{
    public class JwtHelper
    {
        public static string GenerarToken(int idUsuario, string nombre, string correo, IConfiguration config)
        {
            var key = config["Jwt:Key"];
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];
            var expiryMinutes = int.Parse(config["Jwt:ExpiryMinutes"] ?? "1440");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credenciales = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim("Id", idUsuario.ToString()),
                new Claim("name", nombre),
                new Claim(JwtRegisteredClaimNames.Email, correo)
            };

            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expiryMinutes), signingCredentials: credenciales);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
