using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using NuGet.Common;
using System;

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
        public IActionResult Register(int RequestId,string token)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(RegisterVM lc)
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

                    User uc = new User();
                    uc.Email = lc.Email;
                    uc.Aspnetuserid = aspnetUser.Aspnetuserid;
                    uc.Firstname = lc.Email;
                    uc.Createddate=DateTime.Now;
                    uc.Createdby = lc.Email;
                    _context.Users.Add(uc);
                    _context.SaveChanges();

                    return RedirectToAction("Index","Login");
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