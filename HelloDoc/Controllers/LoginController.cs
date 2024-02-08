using AutoMapper;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocPatient.Controllers
{
    public class LoginController : Controller
    {

        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
           

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel a)

        {
            if(ModelState.IsValid)
            {
                var user = await _context.AspnetUsers.FindAsync(a.Username);
                if(user != null) {
                    var LoginDto = new LoginModel
                    {
                        Username = user.Username,
                        Passwordhash = user.Passwordhash,

                    };
                    if(LoginDto.Passwordhash==a.Passwordhash)
                    {
                        return RedirectToAction("Index","Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    }

                }
                else
                {
                    // User not found
                    ModelState.AddModelError(string.Empty, "User not found");
                }

            }
            return View(a);

        }
        

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitFORM(IFormCollection collection)
        {
            return Json(collection);
        }



    }
}
