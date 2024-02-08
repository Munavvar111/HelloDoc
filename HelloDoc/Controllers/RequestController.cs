using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public  IActionResult PatientRequest(RequestModel requestModel)
        {
             requestModel.Username = requestModel.Firstname+requestModel.Lastname;
            var modelStateErrors = this.ModelState.Values.SelectMany(m => m.Errors);
            if (ModelState.IsValid)
            {
                Guid guid = Guid.NewGuid();
                string str = guid.ToString();
                AspnetUser aspnetuser = new AspnetUser();
                aspnetuser.Aspnetuserid = str;
                aspnetuser.Email= requestModel.Email;
                aspnetuser.Username = requestModel.Username;
                _context.AspnetUsers.Add(aspnetuser);
                _context.SaveChanges();

                User user= new User();
                user.Email= requestModel.Email;
                user.Aspnetuserid= aspnetuser.Aspnetuserid;
                user.Firstname= requestModel.Firstname;
                user.Lastname= requestModel.Lastname;   
                user.Street=requestModel.Street;
                user.City= requestModel.City;
                user.State= requestModel.State;
                user.Zipcode=requestModel.Zipcode;
                user.Intyear = requestModel.BirthDate.Year;
                user.Intdate = requestModel.BirthDate.Day;
                user.Strmonth = requestModel.BirthDate.Month.ToString();
                user.Createddate=DateTime.Now;
                _context.Users.Add(user);
                _context.SaveChanges();

                Request request= new Request();
                request.Requesttypeid = 1;
                request.Userid=user.Userid;
                request.Firstname=requestModel.Firstname;
                request.Lastname=requestModel.Lastname; 
                request.Email=requestModel.Email;
                request.Createddate=DateTime.Now;
                request.Status = 1;
                _context.Requests.Add(request); 
                _context.SaveChanges();

                Requestclient requestclient= new Requestclient();   
                requestclient.Requestid = request.Requestid;
                requestclient.Firstname = requestModel.Firstname;
                requestclient.Lastname = requestModel.Lastname;
                requestclient.State= requestModel.State;
                requestclient.Street= requestModel.Street;
                requestclient.City= requestModel.City;
                requestclient.Zipcode=requestModel.Zipcode;

                _context.Requestclients.Add(requestclient); 
                _context.SaveChanges(); 



                return RedirectToAction("Index","Dashboard"); 
            }
            else
            {
                return View(requestModel);
            }
    
        }


        public IActionResult FriendRequest()
        {
            return View();
        }
        public IActionResult ConceirgeRequest()
        {
            return View();
        }


    }
}
