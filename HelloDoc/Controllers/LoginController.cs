using AutoMapper;
using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;

namespace HalloDocPatient.Controllers
{

    public class LoginController : Controller
    {
        private readonly ILogin _login;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(ApplicationDbContext context, ILogin login, IConfiguration configuration)
        {
            _login = login;        
            _context = context;
            _configuration = configuration;
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
                
                    if (_login.isLoginValid(a))
                    {
                var user = await _context.Users.FirstOrDefaultAsync(x=>x.Email==a.Email);
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetInt32("id", user.Userid);
                    HttpContext.Session.SetString("Username", user.Firstname);
                    TempData["ShowToaster"] = true;
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    }
                }

            return View(a);
        }
            

        public IActionResult LogoutController()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Login");
        }


        public IActionResult ForgotPassword()
        {

            return View();
        }
        public IActionResult ResetPassword(int userId, string token)
        {
            return View(new ResetPasswordViewModel { UserId = userId, Token = token });
        }


        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel fm) { 

            if (ModelState.IsValid) {
            var user=_context.Users.Where(x=>x.Email ==fm.Email).FirstOrDefault();
                if (user!=null) { 
                var token=Guid.NewGuid().ToString();
                var resetLink = Url.Action("ResetPassword", "Login", new { userId = user.Userid, token }, protocol: HttpContext.Request.Scheme);

            if(_login.IsSendEmail("jkkikani2003@gmail.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to reset your password."))
                    {

                    return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                         ModelState.AddModelError(string.Empty, "Email Is Not Send");

                    }
                }
                {
                    // Handle user not found
                    ModelState.AddModelError(string.Empty, "User not found");
                }
            }

            return View(); 
        }


        [HttpPost]
        public IActionResult SubmitFORM(IFormCollection collection)
        {
            return Json(collection);
        }



    }
}
