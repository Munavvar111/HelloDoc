using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HalloDocPatient.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOtherRequest _otherrequest;
        private readonly ILogin _login;

        private readonly IPatientRequest _patientRequest;
        private readonly ILogger<RequestController> _logger;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public RequestController(ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider, IPatientRequest patientRequest, ILogger<RequestController> logger, IOtherRequest otherrequest,ILogin login)
        {
            _context = context;
            _patientRequest = patientRequest;
            _logger = logger;
            _login=login;
            _otherrequest = otherrequest;
            _dataProtectionProvider= dataProtectionProvider;    
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

                DataAccessLayer.DataModels.Request request = new DataAccessLayer.DataModels.Request();
                request.Firstname = requestOther.FirstNameOther;
                request.Requesttypeid = 4;//Business 
                request.Lastname = requestOther.LastNameOther;
                request.Email = requestOther.EmailOther;
                request.Phonenumber = requestOther.PhoneNumberOther;
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
                requestclient.Phonenumber = request.Phonenumber;
                requestclient.Requestid = request.Requestid;
                requestclient.Firstname = requestOther.FirstName;
                requestclient.Lastname = requestOther.LastName;
                requestclient.Email = requestOther.Email;
                requestclient.Intdate = requestOther.BirthDate.Day;
                requestclient.Intyear = requestOther.BirthDate.Year;
                requestclient.Strmonth = requestOther.BirthDate.Month.ToString();


                _context.Requestclients.Add(requestclient);
                _context.SaveChanges();
                var token = Guid.NewGuid().ToString();
                var resetLink = Url.Action("Index", "Register", new { userId = request.Requestid, token }, protocol: HttpContext.Request.Scheme);

                if (_login.IsSendEmail("munavvarpopatiya999@gmail.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to Create A new Account"))
                {

                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email Is Not Send");

                }
                return RedirectToAction("Index", "Login");
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
                    var user = _patientRequest.GetUserByEmail(requestModel.Email);
                        
                    var uniqueFileName=await _patientRequest.AddFileInUploader(requestModel.File);
                    _patientRequest.AddPatientRequest(requestModel, ReqTypeId: 1);
                    var request = _patientRequest.GetRequestByEmail(requestModel.Email);
                    _patientRequest.AddRequestWiseFile(uniqueFileName, request.Requestid);
                    
                    return RedirectToAction("Index", "Login");
                }
                //_patientRequest is a interface addpatientrequest is method;
                else {
                    _patientRequest.AddPatientRequest(requestModel, ReqTypeId: 1);
                    var request = _patientRequest.GetRequestByEmail(requestModel.Email);
                    var token = Guid.NewGuid().ToString();
                    var resetLink = Url.Action("Index", "Register", new { userId = request.Requestid, token }, protocol: HttpContext.Request.Scheme);


                    if (_login.IsSendEmail("munavvarpopatiya999@gmail.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to Create A new Account"))
                    {

                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email Is Not Send");

                    }
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
        public async Task<IActionResult> FriendRequest(RequestOthers request)
        {
            if(ModelState.IsValid)
            {
                if (request.File != null && request.File.Length > 0)
                {
                    var uniqueFileName=await _patientRequest.AddFileInUploader(request.File);

                    _otherrequest.AddFriendRequest(request, ReqTypeId: 2);
                    var request1 = _patientRequest.GetRequestByEmail(request.EmailOther);
                    _patientRequest.AddRequestWiseFile(uniqueFileName, request1.Requestid);
                    var token = Guid.NewGuid().ToString();
                    var resetLink = Url.Action("Index", "Register", new { RequestId = request1.Requestid, token }, protocol: HttpContext.Request.Scheme);

                    if (_login.IsSendEmail("munavvarpopatiya999@gmail.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to reset your password."))
                    {

                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email Is Not Send");

                    }
                    return RedirectToAction("Index", "Login");

                }
                else
                {

                    _otherrequest.AddFriendRequest(request, ReqTypeId: 2);
                    var request1 = _patientRequest.GetRequestByEmail(request.EmailOther);
                    var token = Guid.NewGuid().ToString();
                    var resetLink = Url.Action("Index", "Register", new { RequestId = request1.Requestid, token }, protocol: HttpContext.Request.Scheme);

                    if (_login.IsSendEmail("munavvarpopatiya777@outlook.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to reset your password."))
                    {

                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email Is Not Send");

                    }
                    return RedirectToAction("Index", "Login");


                }




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
                var request1 = _patientRequest.GetRequestByEmail(requestOther.EmailOther);

                var token = Guid.NewGuid().ToString();
                var resetLink = Url.Action("Index", "Register", new { RequestId = request1.Requestid, token }, protocol: HttpContext.Request.Scheme);

                if (_login.IsSendEmail("munavvarpopatiya999@gmail.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to reset your password."))
                {

                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email Is Not Send");

                }
                return RedirectToAction("Index", "Login");
            }
            return View(requestOther);
        }

        public IActionResult ReviewAgreement(string requestid)
        {

            // Use the DataProtectionProvider to unprotect the bytes:
            var protector = _dataProtectionProvider.CreateProtector("munavvar");
            string decryptedValue = protector.Unprotect(requestid);


            var requesiddec = int.Parse(decryptedValue);
            var request = _context.Requests.Where(item => item.Requestid == requesiddec).FirstOrDefault();
            var sendagrement = new AgreementVM();
            sendagrement.status = request.Status;
            sendagrement.RequestId = request.Requestid;
            if (request.Status == 2)
            {

            return View(sendagrement);
            }
            else
            {
                TempData["ToasterMessage"] = "Request status is not 2. Please log in to continue.";
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public IActionResult Agree(int id)
        {
            var request = _context.Requests.Where(item => item.Requestid == id).FirstOrDefault();
            request.Status = 4;
            _context.Update(request);
            _context.SaveChanges();
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public IActionResult CancelPatient(AgreementVM ag)
        {
            var request = _context.Requests.Where(item=>item.Requestid==ag.RequestId).FirstOrDefault();
            request.Status = 7;
            _context.Requests.Update(request);
            var requeststatuslog = new Requeststatuslog();
            requeststatuslog.Status = 7;
            requeststatuslog.Createddate=DateTime.Now;
            requeststatuslog.Notes = ag.Notes;
            requeststatuslog.Requestid = ag.RequestId;
            _context.Requeststatuslogs.Add(requeststatuslog);
            _context.SaveChanges();

            return RedirectToAction("Index", "Login");
        }

    }
}
