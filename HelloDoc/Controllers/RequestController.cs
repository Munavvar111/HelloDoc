using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace HalloDocPatient.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOtherRequest _otherrequest;

        private readonly IPatientRequest _patientRequest;
        private readonly ILogger<RequestController> _logger;

        public RequestController(ApplicationDbContext context, IPatientRequest patientRequest, ILogger<RequestController> logger, IOtherRequest otherrequest)
        {
            _context = context;
            _patientRequest = patientRequest;
            _logger = logger;
            _otherrequest = otherrequest;
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

        //PaatientRequest

        [HttpPost]
        public  async Task<IActionResult> PatientRequest( RequestModel requestModel)
        {
            requestModel.Username = requestModel.Firstname+requestModel.Lastname;
            if(!ModelState.IsValid)
            {
                return View(requestModel);
            }
            else 
            {
                if(requestModel.File!=null && requestModel.File.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + requestModel.File.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await requestModel.File.CopyToAsync(stream);
                    }
                    _patientRequest.AddPatientRequest(requestModel, ReqTypeId: 1);
                    var request = _patientRequest.GetRequestByEmail(requestModel.Email);
                    _patientRequest.AddRequestWiseFile(uniqueFileName, request.Requestid);
                    return RedirectToAction("Index", "Login");
                }
                //_patientRequest is a interface addpatientrequest is method;
                else {
                    _patientRequest.AddPatientRequest(requestModel, ReqTypeId: 1);
                    return RedirectToAction("Index", "Login");

                }
            }
            
    
        }
        public IActionResult Error()
        {
            // Retrieve the error message from TempData
            var errorMessage = TempData["ErrorMessage"] as string;

            // You can also pass the error message to the view
            ViewBag.ErrorMessage = errorMessage;

            return View("Error");
        }
        [HttpPost]
        public JsonResult CheckEmail([FromBody] string email)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == email);
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
                _otherrequest.AddFriendRequest(request, ReqTypeId: 2);
                 return RedirectToAction("Index", "Dashboard");




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
                _otherrequest.AddConceirgeRequest(requestOther, ReqTypeId: 3);

                return RedirectToAction("Index", "dashboard");
            }
            return View(requestOther);
        }

    }
}
