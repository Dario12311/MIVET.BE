using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MIVET.BE.Transversales;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MedicalWeb.BE.Transversales.Encriptacion
{
    public class JwtConfiguration
    {
        public static string GetToken(UsuarioDTO usuarioInfo, IConfiguration config)
        {
            string secretKey = config["Jwt:SecretKey"];
            string issuer = config["Jwt:Issuer"];
            string audience = config["Jwt:Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var roles = (usuarioInfo.RolId ?? string.Empty).Split(',');

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuarioInfo.NombreUsuario),
                new Claim("UsuarioId", usuarioInfo.Identificacion.ToString())
            };

            foreach (var rol in roles)
            {
                claims.Add(new Claim("roles", rol.Trim()));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static int GetTokenIdUsuario(ClaimsIdentity identity)
        {
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type == "UsuarioId")
                    {
                        return int.Parse(claim.Value);
                    }
                }
            }
            return 0;
        }
    }
}