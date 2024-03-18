using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.DataProtection;
using DataAccessLayer.DataModels;

namespace HalloDocPatient.Controllers
{

    public class LoginController : Controller
    {
        private readonly ILogin _login;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtAuth _jwtAuth;


        public LoginController(ApplicationDbContext context, ILogin login, IConfiguration configuration, IJwtAuth jwtAuth, IDataProtectionProvider dataProtectionBuilder)
        {
            _login = login;
            _context = context;
            _configuration = configuration;
            _jwtAuth = jwtAuth;
        }



        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel a)
        {
            if (ModelState.IsValid)
            {
                if (_login.isLoginValid(a))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == a.Email);
                    AspnetUser? aspnetuser = _context.AspnetUsers.FirstOrDefault(x => x.Email == a.Email);
                    if (aspnetuser != null)
                    {
                        AspnetUserrole? role = _context.AspnetUserroles.FirstOrDefault(x => aspnetuser.Aspnetuserid == x.Userid);
                        if (role != null)
                        {

                            AspnetRole? rolefromroleid = _context.AspnetRoles.FirstOrDefault(x => x.Id == role.Roleid);

                            var token = _jwtAuth.GenerateToken(user?.Email ?? "", rolefromroleid?.Name ?? "".Trim());
                            HttpContext.Session.SetString("Role", rolefromroleid?.Name ?? "".Trim());
                            HttpContext.Session.SetString("token", token);
                            HttpContext.Session.SetString("Email", user?.Email ?? "");
                            HttpContext.Session.SetInt32("id", user?.Userid ?? 1);
                            HttpContext.Session.SetString("Username", user?.Firstname ?? "");
                            if (rolefromroleid?.Name.Trim() == "admin")
                            {
                                TempData["SuccessMessage"] = "Login  successful!";
                                return RedirectToAction("Index", "Admin");
                            }
                            else if (rolefromroleid?.Name.Trim() == "user")
                            {
                                TempData["SuccessMessage"] = "Login  successful!";

                                return RedirectToAction("Index", "Dashboard");
                            }

                        }
                        else
                        {
                            TempData["Error"] = "Role Is Not Defind";
                            return RedirectToAction("Index", "Login");
                        }
                    }
                    else
                    {
                        TempData["Error"] = "User NotFound";
                        return RedirectToAction("Index", "Login");
                    }
                }

                else
                {
                    TempData["Error"] = "Login  Unsuccessful!";

                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                }
            }

            return View(a);
        }


        public IActionResult LogoutController()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }


        public IActionResult ForgotPassword()
        {

            return View();
        }
        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordViewModel { UserId = userId, Token = token });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            var user = _context.AspnetUsers.Where(x => x.Aspnetuserid == model.UserId).FirstOrDefault();
            if (ModelState.IsValid && user!=null)
            {
                if (model.Password == model.ConfirmPassword)
                {
                    user.Passwordhash = BC.HashPassword(model.Password);
                    _context.Update(user);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Login");
                }

            }
            return View(model);
        }
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel fm)
        {

            if (ModelState.IsValid)
            {
                var user = _context.AspnetUsers.Where(x => x.Email == fm.Email).FirstOrDefault();
                if (user != null)
                {
                    var token = Guid.NewGuid().ToString();
                    var resetLink = Url.Action("ResetPassword", "Login", new { userId = user.Aspnetuserid, token }, protocol: HttpContext.Request.Scheme);

                    if (_login.IsSendEmail("munavvarpopatiya999@gmail.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to reset your password."))
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

        public string Ajaxlogout()
        {
            HttpContext.Session.Clear();
            return "<script>window.loction.href='/Login'</script>";
        }




    }
}
