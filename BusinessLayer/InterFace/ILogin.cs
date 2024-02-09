using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.CustomModel;

namespace BusinessLayer.InterFace
{
    public interface ILogin
    {
       public bool isLoginValid(LoginModel a);
    }
}
