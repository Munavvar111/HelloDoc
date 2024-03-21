using BusinessLayer.InterFace;
using DataAccessLayer.DataModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repository
{

    public class JwtAuthRepo : IJwtAuth
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secretKey;
        public IConfiguration Configuration { get; }
        public JwtAuthRepo(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string GenerateToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
     {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, role)
    }),
                Expires = DateTime.UtcNow.AddMinutes(Double.Parse(Configuration["Jwt:ExpiryDays"])),
                SigningCredentials = credentials
            };


            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }


        public bool ValidateToken(string token, out JwtSecurityToken jwttoken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);
                jwttoken = (JwtSecurityToken)validatedToken;

                if (jwttoken != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                jwttoken = null;
                return false;
            }


        }
    }
}
