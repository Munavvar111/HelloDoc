using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

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

        public RequestController(ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider, IPatientRequest patientRequest, ILogger<RequestController> logger, IOtherRequest otherrequest, ILogin login)
        {
            _context = context;
            _patientRequest = patientRequest;
            _logger = logger;
            _login = login;
            _otherrequest = otherrequest;
            _dataProtectionProvider = dataProtectionProvider;
        }
        public IActionResult PatientRequest()
        {
            List<Region> region = _context.Regions.ToList();
            RequestModel requestmodal = new RequestModel();
            requestmodal.Regions = region;
            return View(requestmodal);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BusinessRequest()
        {
            List<Region> region = _context.Regions.ToList();
            RequestOthers requestmodal = new RequestOthers();
            requestmodal.Regions = region;
            return View(requestmodal);
        }

        [HttpPost]
        public IActionResult BusinessRequest(RequestOthers requestOther)
        {
            Region? statebyregionid = _context.Regions.Where(item => item.Name == requestOther.State).FirstOrDefault();
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
                request.Requesttypeid = 4;
                request.Lastname = requestOther.LastNameOther;
                request.Email = requestOther.EmailOther;
                request.Phonenumber = requestOther.PhoneNumberOther;
                request.Status = 1;
                request.Createddate = DateTime.Now;
                request.Isdeleted = false;

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
                requestclient.City = requestOther.City;
                requestclient.State = requestOther.State;
                requestclient.Street = requestOther.Street;
                requestclient.Zipcode = requestOther.Zipcode;
                requestclient.Regionid = statebyregionid?.Regionid;


                _context.Requestclients.Add(requestclient);
                _context.SaveChanges();
                Region? region = _context.Regions.Where(x => x.Name == requestOther.State).FirstOrDefault();
                int count = _context.Requests.Where(x => x.Createddate.Date == request.Createddate.Date).Count() + 1;
                if (region != null)
                {
                    string confirmNum = string.Concat(region?.Abbreviation ?? "".ToUpper(), request.Createddate.ToString("ddMMyy"), requestOther.LastName.Substring(0, 2).ToUpper() ?? "",
                   requestOther.FirstName.Substring(0, 2).ToUpper(), count.ToString("D4"));
                    request.Confirmationnumber = confirmNum;
                }
                else
                {
                    string confirmNum = string.Concat("ML", request.Createddate.ToString("ddMMyy"), requestOther.LastName.Substring(0, 2).ToUpper() ?? "",
                  requestOther.FirstName.Substring(0, 2).ToUpper(), count.ToString("D4"));
                    request.Confirmationnumber = confirmNum;
                }
                string token = Guid.NewGuid().ToString();
                string? resetLink = Url.Action("Index", "Register", new { userId = request.Requestid, token }, protocol: HttpContext.Request.Scheme);

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
        public async Task<IActionResult> PatientRequest(RequestModel requestModel)
        {
            requestModel.Username = requestModel.Firstname + requestModel.Lastname;
            Blockrequest? blockrequest = _context.Blockrequests.Where(item => item.Email == requestModel.Email).FirstOrDefault();
            if (blockrequest == null)
            {
                if (!ModelState.IsValid)
                {
                    return View(requestModel);
                }
                else
                {
                    if (requestModel.File != null && requestModel.File.Length > 0)
                    {
                        string uniqueFileName = await _patientRequest.AddFileInUploader(requestModel.File);
                        _patientRequest.AddPatientRequest(requestModel, ReqTypeId: 1);
                        Request? request = _patientRequest.GetRequestByEmail(requestModel.Email);
                        User? user = _patientRequest.GetUserByEmail(requestModel.Email);
                        
                        if (user == null && request != null)
                        {
                            string token = Guid.NewGuid().ToString();
                            string? resetLink = Url.Action("Index", "Register", new { userId = request.Requestid, token }, protocol: HttpContext.Request.Scheme);
                            
                            if (_login.IsSendEmail("munavvarpopatiya999@gmail.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to Create A new Account"))
                                return RedirectToAction("Index", "Login");
                            else
                                return RedirectToAction("Index", "Login");
                        }
                        _patientRequest.AddRequestWiseFile(uniqueFileName, request?.Requestid ?? 1);
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        _patientRequest.AddPatientRequest(requestModel, ReqTypeId: 1);
                        Request? request = _patientRequest.GetRequestByEmail(requestModel.Email);
                        string token = Guid.NewGuid().ToString();
                        string? resetLink = Url.Action("Index", "Register", new { userId = request?.Requestid ?? 1, token }, protocol: HttpContext.Request.Scheme);
                        
                        if (_login.IsSendEmail("munavvarpopatiya999@gmail.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to Create A new Account"))
                            return RedirectToAction("Index", "Login");
                        else
                            ModelState.AddModelError(string.Empty, "Email Is Not Send");
                        
                        return RedirectToAction("Index", "Login");
                    }
                }
            }
            else
            {
                TempData["Error"] = "Your Request IS Block Beacuse Of Your Unaporpriate Behivour!";
                return RedirectToAction("PatientRequest");
            }


        }
        public IActionResult Error()
        {
            // Retrieve the error message from TempData
            string? errorMessage = TempData["ErrorMessage"] as string;

            // You can also pass the error message to the view
            ViewBag.ErrorMessage = errorMessage;

            return View("Error");
        }
        [HttpPost]
        public JsonResult CheckEmail([FromBody] string email)
        {
            AspnetUser? user = _context.AspnetUsers.FirstOrDefault(u => u.Email == email);
            bool isValid = user == null;
            return Json(isValid);
        }
        public IActionResult FriendRequest()
        {
            List<Region> region = _context.Regions.ToList();
            RequestOthers requestmodal = new RequestOthers();
            requestmodal.Regions = region;
            return View(requestmodal);

        }

        [HttpPost]
        public async Task<IActionResult> FriendRequest(RequestOthers request)
        {
            if (ModelState.IsValid)
            {
                if (request.File != null && request.File.Length > 0)
                {
                    string uniqueFileName = await _patientRequest.AddFileInUploader(request.File);
                    _otherrequest.AddFriendRequest(request, ReqTypeId: 2);
                    Request? request1 = _patientRequest.GetRequestByEmail(request.EmailOther);
                    _patientRequest.AddRequestWiseFile(uniqueFileName, request1?.Requestid ?? 1);
                    string? token = Guid.NewGuid().ToString();
                    string? resetLink = Url.Action("Index", "Register", new { RequestId = request1?.Requestid ?? 1, token }, protocol: HttpContext.Request.Scheme);

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
                    Request? request1 = _patientRequest.GetRequestByEmail(request.EmailOther);
                    string? token = Guid.NewGuid().ToString();
                    string? resetLink = Url.Action("Index", "Register", new { RequestId = request1?.Requestid ?? 1, token }, protocol: HttpContext.Request.Scheme);

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
            List<Region> region = _context.Regions.ToList();
            RequestOthers requestmodal = new RequestOthers();
            ViewBag.Regions = region;
            return View(requestmodal);
        }
        [HttpPost]
        public IActionResult ConceirgeRequest(RequestOthers requestOther)
        {
            if (ModelState.IsValid)
            {
                _otherrequest.AddConceirgeRequest(requestOther, ReqTypeId: 3);
                Request? request1 = _patientRequest.GetRequestByEmail(requestOther.EmailOther);
                string? token = Guid.NewGuid().ToString();
                string? resetLink = Url.Action("Index", "Register", new { RequestId = request1?.Requestid ?? 1, token }, protocol: HttpContext.Request.Scheme);
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
            List<Region> region = _context.Regions.ToList();

            ViewBag.Regions = region;

            return View(requestOther);
        }

        public IActionResult ReviewAgreement(string requestid)
        {
            try
            {
                var protector = _dataProtectionProvider.CreateProtector("munavvar");
                string decryptedValue = protector.Unprotect(requestid);


                int requesiddec = int.Parse(decryptedValue);
                Request? request = _context.Requests.Where(item => item.Requestid == requesiddec).FirstOrDefault();
                if (request != null)
                {
                    AgreementVM sendagrement = new AgreementVM();
                    sendagrement.status = request.Status;
                    sendagrement.RequestId = request.Requestid;
                    if (request.Status == 2)
                    {
                        return View(sendagrement);
                    }
                    else
                    {
                        TempData["Error"] = "Request status is not 2. Please log in to continue.";
                        return RedirectToAction("Index", "Login");
                    }
                }
                else
                {
                    TempData["Error"] = "Request Is Not Found";
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TempData["Error"] = "You Cant Change The Url";
                return RedirectToAction("Index", "Login");

            }
        }

        [HttpPost]
        public IActionResult Agree(int id)
        {
            try
            {
                Request? request = _context.Requests.Where(item => item.Requestid == id).FirstOrDefault();
                if (request == null)
                {
                    return NotFound();
                }

                request.Status = 4;
                _context.Update(request);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Agreement completed successfully";

                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPost]
        public IActionResult CancelPatient(AgreementVM ag)
        {
            Request? request = _context.Requests.Where(item => item.Requestid == ag.RequestId).FirstOrDefault();
            if (request != null)
            {
                request.Status = 7;
                _context.Requests.Update(request);
                Requeststatuslog requeststatuslog = new Requeststatuslog();
                requeststatuslog.Status = 7;
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Notes = ag.Notes;
                requeststatuslog.Requestid = ag.RequestId;
                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Login");
        }

    }
}
