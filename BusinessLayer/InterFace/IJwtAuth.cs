using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.InterFace
{
    public interface IJwtAuth
    {
        public string GenerateToken(string username, string role);

        public bool ValidateToken(string token, out JwtSecurityToken jwttoken);
    }
}
