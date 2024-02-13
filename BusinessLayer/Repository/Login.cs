using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;
using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;

namespace BusinessLayer.Repository
{
    public class Login : ILogin
    {   
        private readonly ApplicationDbContext _context;
        public Login(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool isLoginValid(LoginModel a)
        {
            var account = _context.AspnetUsers.SingleOrDefault(x => x.Email == a.Email);

            // check account found and verify password
            if (account == null || !BC.Verify(a.Passwordhash, account.Passwordhash))
            {
                // authentication failed
                return false;
            }
            else
            {
                // authentication successful
                return true;
            }
        }
    }
}
