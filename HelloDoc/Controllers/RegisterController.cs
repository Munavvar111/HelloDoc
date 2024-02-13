using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace HalloDocPatient.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult View1()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginModel lc)
        {
            if (ModelState.IsValid)
            {
                if (lc.Passwordhash == lc.ConfirmPasswordhash)
                {


                    AspnetUser aspnetUser = new AspnetUser();
                    aspnetUser.Passwordhash = BC.HashPassword(lc.Passwordhash);

                    aspnetUser.Aspnetuserid = lc.Email;
                    aspnetUser.Username = lc.Email;
                    aspnetUser.Email=lc.Email;    
                    _context.AspnetUsers.Add(aspnetUser);
                    _context.SaveChanges();

                    

                    return RedirectToAction(nameof(View1));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Password is Not Match");
                    return View(lc);  
                }
            }


            else { return View(lc); }

        }

    }
}