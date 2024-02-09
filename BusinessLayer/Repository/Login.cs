using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return _context.AspnetUsers.Any(i => i.Username == a.Username && i.Passwordhash == a.Passwordhash);
        }
    }
}
