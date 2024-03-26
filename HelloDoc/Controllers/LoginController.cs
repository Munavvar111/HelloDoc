using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.DataProtection;
using DataAccessLayer.DataModels;
using Newtonsoft.Json;

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
                    var admin=_context.Admins.FirstOrDefault(x=>x.Email==a.Email);
                    AspnetUser? aspnetuser = _context.AspnetUsers.FirstOrDefault(x => x.Email == a.Email);
                    if (aspnetuser != null)
                    {
                        AspnetUserrole? role = _context.AspnetUserroles.FirstOrDefault(x => aspnetuser.Aspnetuserid == x.Userid);
                        if (role != null)
                        {

                            Role? rolefromroleid = _context.Roles.FirstOrDefault(x => x.Roleid == role.Roleid);
                            var rolemenu = _context.Rolemenus.Where(item => item.Roleid == rolefromroleid.Roleid).Select(item => item.Menuid).ToList();
                            var token = _jwtAuth.GenerateToken(user?.Email ?? "", rolefromroleid.Roleid.ToString());
                            HttpContext.Session.SetString("Role", rolefromroleid.Roleid.ToString());
                            HttpContext.Session.SetString("token", token);
                            HttpContext.Session.SetString("aspnetid", aspnetuser.Aspnetuserid);
                            HttpContext.Session.SetString("UserPermissions", JsonConvert.SerializeObject(rolemenu));
                            HttpContext.Session.SetString("Email", a?.Email ?? "");
                            HttpContext.Session.SetInt32("id", user?.Userid ?? 1);
                            HttpContext.Session.SetString("Username", user?.Firstname ?? "");
                            if (admin!=null)
                            {
                                TempData["SuccessMessage"] = "Login  successful!";
                                return RedirectToAction("Index", "Admin");
                            }
                            else if (user!=null)
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
            return Ok(collection);
        }

        public string Ajaxlogout()
        {
            HttpContext.Session.Clear();
            return "<script>window.loction.href='/Login'</script>";
        }




    }
}
