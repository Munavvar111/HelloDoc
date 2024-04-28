using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using NuGet.Common;
using System;
using BusinessLayer.InterFace;

namespace HalloDocPatient.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdmin _admin;

        public RegisterController(ApplicationDbContext context,IAdmin admin)
        {
            _context = context;
            _admin = admin;
        }
        public IActionResult Index(int RequestId, string token)
        {
            ViewBag.RequestId = RequestId;
            ViewBag.Token = token;
            Requestclient requestclient = _admin.GetRequestclientByRequestId(RequestId);
            ViewBag.Email = requestclient.Email;
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
        public IActionResult Index(RegisterVM lc,int RequestId,string token)
        {
            Request request = _admin.GetRequestById(RequestId);
            Requestclient requestclient=_admin.GetRequestclientByRequestId(RequestId);
            lc.Email = requestclient.Email;
            AspnetUser? aspnetUser1 = _context.AspnetUsers.Where(item => item.Email == request.Email).FirstOrDefault();
            if (request == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else if (aspnetUser1 != null) {
				return RedirectToAction("Index", "Login");

			}
			if (ModelState.IsValid)
            {
                if ((lc.Passwordhash == lc.ConfirmPasswordhash) && request!=null)
                {


                    AspnetUser aspnetUser = new AspnetUser();
                    aspnetUser.Passwordhash = BC.HashPassword(lc.Passwordhash);

                    aspnetUser.Aspnetuserid = request.Email;
                    aspnetUser.Username = requestclient.Firstname;
                    aspnetUser.Phonenumber = requestclient.Phonenumber;
                    aspnetUser.Email=requestclient.Email;    
                   
                    _context.AspnetUsers.Add(aspnetUser);
                    _context.SaveChanges();

                    AspnetUserrole aspnetUserrole=new AspnetUserrole();
                    aspnetUserrole.Userid = aspnetUser.Aspnetuserid;
                    aspnetUserrole.Roleid = 2;
                    _context.AspnetUserroles.Add(aspnetUserrole); 
                    _context.SaveChanges();

                    User uc = new User();
                    uc.Email =requestclient.Email;
                    uc.Aspnetuserid = aspnetUser.Aspnetuserid;
                    uc.Firstname = requestclient.Firstname;
                    uc.Lastname = requestclient.Lastname;
                    uc.Status = 1;
                    uc.Street = requestclient.Street;   
                    uc.City = requestclient.City;   
                    uc.Regionid = requestclient.Regionid;
                    uc.Createddate = DateTime.Now;
                    uc.Createddate=DateTime.Now;
                    uc.Createdby = aspnetUser.Aspnetuserid;
                    uc.Zipcode = requestclient.Zipcode;
                    uc.Isdeleted = new System.Collections.BitArray(new[] {false});
                    uc.Intdate = requestclient.Intdate;
                    uc.Intyear= requestclient.Intyear;
                    uc.Strmonth = requestclient.Strmonth;
                    uc.Mobile = requestclient.Phonenumber;

                    _context.Users.Add(uc);
                    _context.SaveChanges();

                    request.Userid = uc.Userid;
                    _admin.UpdateRequest(request);
                    _admin.SaveChanges();
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