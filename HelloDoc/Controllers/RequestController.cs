using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace HalloDocPatient.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult PatientRequest()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BusinessRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BusinessRequest(RequestOthers requestOther)
        {
            if (ModelState.IsValid)
            {
                Business business = new Business();
                business.Name = requestOther.FirstNameOther;
                business.City = requestOther.City;
                business.Zipcode = requestOther.Zipcode;
                business.Createddate = DateTime.Now;
                business.Regionid = 1;

                _context.Businesses.Add(business);
                _context.SaveChanges();

                Request request = new Request();
                request.Firstname = requestOther.FirstNameOther;
                request.Requesttypeid = 2;//Friend 
                request.Lastname = requestOther.LastNameOther;
                request.Email = requestOther.EmailOther;
                request.Status = 1;//Unsigned
                request.Createddate = DateTime.Now;

                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestbusiness requestbusiness = new Requestbusiness();
                requestbusiness.Businessid = business.Businessid;
                requestbusiness.Requestid = request.Requestid;

                _context.Requestbusinesses.Add(requestbusiness);
                _context.SaveChanges();


                Requestclient requestclient = new Requestclient();
                requestclient.Requestid = request.Requestid;
                requestclient.Firstname = requestOther.FirstName;
                requestclient.Lastname = requestOther.LastName;
                requestclient.Email = requestOther.Email;
                requestclient.Intdate = requestOther.BirthDate.Day;
                requestclient.Intyear = requestOther.BirthDate.Year;
                requestclient.Strmonth = requestOther.BirthDate.Month.ToString();


                _context.Requestclients.Add(requestclient);
                _context.SaveChanges();

                return RedirectToAction("Index", "dashboard");
            }
            return View(requestOther);
        }


        [HttpPost]
        public  async Task<IActionResult> PatientRequest(RequestModel requestModel)
        {
             requestModel.Username = requestModel.Firstname+requestModel.Lastname;
            var modelStateErrors = this.ModelState.Values.SelectMany(m => m.Errors);
            if (ModelState.IsValid)
            {
                Guid guid = Guid.NewGuid();
                string str = guid.ToString();
                var email = _context.AspnetUsers.FirstOrDefault(u => u.Email == requestModel.Email);
                if (email != null)
                {
                    var User= _context.Users.FirstOrDefault(u=>u.Email==requestModel.Email);
                    Request request = new Request();
                    request.Requesttypeid = 1;
                    request.Userid =User.Userid;
                    request.Firstname = requestModel.Firstname;
                    request.Lastname = requestModel.Lastname;
                    request.Email = requestModel.Email;
                    request.Createddate = DateTime.Now;
                    request.Status = 1;
                    _context.Requests.Add(request);
                    _context.SaveChanges();

                    Requestclient requestclient = new Requestclient();
                    requestclient.Requestid = request.Requestid;
                    requestclient.Firstname = requestModel.Firstname;
                    requestclient.Lastname = requestModel.Lastname;
                    requestclient.State = requestModel.State;
                    requestclient.Street = requestModel.Street;
                    requestclient.City = requestModel.City;
                    requestclient.Zipcode = requestModel.Zipcode;

                    _context.Requestclients.Add(requestclient);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Dashboard");

                }
                else
                {


                    AspnetUser aspnetuser = new AspnetUser();
                    aspnetuser.Aspnetuserid = str;
                    aspnetuser.Email = requestModel.Email;
                    aspnetuser.Username = requestModel.Username;
                    _context.AspnetUsers.Add(aspnetuser);
                    _context.SaveChanges();

                    User user = new User();
                    user.Email = requestModel.Email;
                    user.Aspnetuserid = aspnetuser.Aspnetuserid;
                    user.Firstname = requestModel.Firstname;
                    user.Lastname = requestModel.Lastname;
                    user.Street = requestModel.Street;
                    user.City = requestModel.City;
                    user.State = requestModel.State;
                    user.Zipcode = requestModel.Zipcode;
                    user.Intyear = requestModel.BirthDate.Year;
                    user.Intdate = requestModel.BirthDate.Day;
                    user.Strmonth = requestModel.BirthDate.Month.ToString();
                    user.Createddate = DateTime.Now;
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    Request request = new Request();
                    request.Requesttypeid = 1;
                    request.Userid = user.Userid;
                    request.Firstname = requestModel.Firstname;
                    request.Lastname = requestModel.Lastname;
                    request.Email = requestModel.Email;
                    request.Createddate = DateTime.Now;
                    request.Status = 1;
                    _context.Requests.Add(request);
                    _context.SaveChanges();

                    Requestclient requestclient = new Requestclient();
                    requestclient.Requestid = request.Requestid;
                    requestclient.Firstname = requestModel.Firstname;
                    requestclient.Lastname = requestModel.Lastname;
                    requestclient.State = requestModel.State;
                    requestclient.Street = requestModel.Street;
                    requestclient.City = requestModel.City;
                    requestclient.Zipcode = requestModel.Zipcode;

                    _context.Requestclients.Add(requestclient);
                    _context.SaveChanges();



                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else
            {
                return View(requestModel);
            }
    
        }
        [HttpPost]
        public JsonResult CheckEmail([FromBody] string email)
        {
            AspnetUser user = _context.AspnetUsers.FirstOrDefault(u => u.Email == email);
            bool isValid = user == null;
            return Json(isValid);
        }

        public IActionResult FriendRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FriendRequest(RequestOthers request)
        {
            if(ModelState.IsValid)
            {
                Request friend=new Request();
                friend.Requesttypeid = 2;//Friend 
                friend.Firstname = request.FirstNameOther;
                friend.Lastname = request.LastNameOther;
                friend.Email = request.EmailOther;
                friend.Status = 1;//Unsigned
                friend.Relationname = request.Relation;
                friend.Createddate = DateTime.Now;
                _context.Requests.Add(friend);  
                _context.SaveChanges();

                Requestclient requestclient = new Requestclient();
                requestclient.Requestid=friend.Requestid;
                requestclient.Firstname=request.FirstName;
                requestclient.Lastname= request.LastName;  
                requestclient.Email= request.Email;
                requestclient.Intdate = request.BirthDate.Day;
                requestclient.Intyear=request.BirthDate.Year;
                requestclient.Strmonth = request.BirthDate.Month.ToString();
                requestclient.Street = request.Street;
                requestclient.City = request.City;  
                requestclient.State = request.State;    
                requestclient.Zipcode = request.Zipcode;
                _context.Requestclients.Add(requestclient);
                _context.SaveChanges();


            }
            return View(request);
        }
            public IActionResult ConceirgeRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ConceirgeRequest(RequestOthers requestOther)
        {
            if (ModelState.IsValid)
            {
                Concierge concierge = new Concierge();
                concierge.Conciergename = requestOther.FirstNameOther;
                concierge.City = requestOther.City;
                concierge.State = requestOther.State;    
                concierge.Street=requestOther.Street;
                concierge.Zipcode=requestOther.Zipcode;
                concierge.Createddate=DateTime.Now;
                concierge.Regionid = 1;

                _context.Concierges.Add(concierge);
                _context.SaveChanges();

                Request request = new Request();
                request.Firstname = requestOther.FirstNameOther;
                request.Requesttypeid = 2;//Friend 
                request.Lastname = requestOther.LastNameOther;
                request.Email = requestOther.EmailOther;
                request.Status = 1;//Unsigned
                request.Createddate = DateTime.Now;
                
                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestconcierge requestconcierge= new Requestconcierge();
                requestconcierge.Conciergeid = concierge.Conciergeid;
                requestconcierge.Requestid=request.Requestid;

                _context.Requestconcierges.Add(requestconcierge);
                _context.SaveChanges();


                Requestclient requestclient = new Requestclient();
                requestclient.Requestid = request.Requestid;
                requestclient.Firstname = requestOther.FirstName;
                requestclient.Lastname = requestOther.LastName;
                requestclient.Email = requestOther.Email;
                requestclient.Intdate = requestOther.BirthDate.Day;
                requestclient.Intyear = requestOther.BirthDate.Year;
                requestclient.Strmonth = requestOther.BirthDate.Month.ToString();


                _context.Requestclients.Add(requestclient);
                _context.SaveChanges();

                return RedirectToAction("Index", "dashboard");
            }
            return View(requestOther);
        }

    }
}
