﻿using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocPatient.Controllers
{
    [CustomAuthorize("Dashboard", "24")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _patientRequest;
        private readonly IAdmin _admin;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DashboardController(ApplicationDbContext context, IPatientRequest patientRequest, IWebHostEnvironment hostingEnvironment, IAdmin admin)
        {
            _context = context;
            _patientRequest = patientRequest;
            _hostingEnvironment = hostingEnvironment;
            _admin = admin;
        }
        #region ViewDocument
        public async Task<IActionResult> ViewDocument(int requestid)
        {
            List<Requestwisefile> reqfile = await _patientRequest.GetRequestwisefileByIdAsync(requestid);

            List<Requestwisefile> reqfiledeleted = reqfile.Where(item => item.Isdeleted != null && (item.Isdeleted.Length == 0 || !item.Isdeleted[0])).ToList();


            int? id = HttpContext.Session.GetInt32("id");
            if (id.HasValue)
            {

                User? user = _patientRequest.GetUserById(id.GetValueOrDefault());
                ViewData["Name"] = user?.Firstname;
                RequestFileViewModel viewmodel = new RequestFileViewModel
                {
                    User = user,
                    Requestid = requestid,
                    Requestwisefileview = reqfiledeleted
                };
                return View(viewmodel);
            }
            else
            {
                TempData["Error"] = "Something Went Wrong";
                return RedirectToAction("Index", "Login");
            }
        }
        #endregion

        #region Profile
        public IActionResult Profile()
        {
            int? id = HttpContext.Session.GetInt32("id");
            User? User = _patientRequest.GetUserById((int)id);
            DateOnly BirthDate = User != null && User.Intyear != null && User.Strmonth != null && User.Intdate != null
            ? new DateOnly((int)User.Intyear, int.Parse(User.Strmonth), (int)User.Intdate)
                                                                    : new DateOnly();
            var regions = _admin.GetAllRegion();
            ViewBag.BirthDate = BirthDate.ToString("yyyy-MM-dd");
            ProfileVM profildata = new ProfileVM();
            profildata.Email = User?.Email ?? "";
            profildata.BirthDate = BirthDate;
            profildata.PhoneNO = User?.Mobile ?? "";
            profildata.FirstName = User?.Firstname ?? "";
            profildata.State = User?.State ?? "";
            profildata.City = User?.City ?? "";
            profildata.Street = User?.Street ?? "";
            profildata.ZipCode = User?.Zipcode ?? "";
            profildata.LastName = User?.Lastname ?? "";
            profildata.Regions = regions;
            ViewBag.State = User?.State ?? "";
            ViewBag.City = User?.City ?? "";
            ViewBag.Street = User?.Street ?? "";

            return View(profildata);
        }
        #endregion

        #region ProfilePost
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            var statebyregionid = _admin.GetRegionByName(profileVM.State);
            int? id = HttpContext.Session.GetInt32("id");
            User? User = _patientRequest.GetUserById((int)id);
            ViewData["Name"] = User?.Firstname ?? "";

            if (ModelState.IsValid)
            {
                if (User != null)
                {

                    User.Firstname = profileVM.FirstName;
                    User.Lastname = profileVM.LastName;
                    User.Street = profileVM.Street;
                    User.City = profileVM.City;
                    User.State = profileVM.State;
                    User.Mobile = profileVM.PhoneNO;
                    User.Zipcode = profileVM.ZipCode;
                    User.Intdate = profileVM.BirthDate.Day;
                    User.Intyear = profileVM.BirthDate.Year;
                    User.Strmonth = profileVM.BirthDate.Month.ToString();
                    User.Regionid = statebyregionid?.Regionid;

                    _context.Update(User);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Profile");
            }
            else
            {
                return View(profileVM);
            }
        }
        #endregion

        #region uploadFile
        [HttpPost]
        public async Task<IActionResult> UploadFile(RequestFileViewModel rm, int requestid)
        {
            int id = requestid;

            if (rm.File != null)
            {
                string uniqueFileName = await _patientRequest.AddFileInUploader(rm.File);
                _patientRequest.AddRequestWiseFile(uniqueFileName, requestid);
                return RedirectToAction("ViewDocument", "Dashboard", new { requestid = requestid });
            }
            else
            {
                return RedirectToAction("ViewDocument", "Dashboard", new { requestid = requestid });

            }
        }
        #endregion

        #region ViewFile
        public IActionResult ViewFile(string filename)
        {
            string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot/uploads", filename);

            if (System.IO.File.Exists(filePath))
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                // Determine MIME type based on file extension
                string contentType;
                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                    case ".doc":
                        contentType = "application/msword";
                        break;
                    case ".docx":
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case ".jpg":
                    case ".jpeg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".jfif":
                        contentType = "image/jfif";
                        break;
                    // Add more cases for other file types as needed
                    default:
                        contentType = "application/octet-stream"; // Default to binary data
                        break;
                }

                // Return the file with appropriate MIME type
                return File(fileStream, contentType);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region Index
        public IActionResult Index()
        {

            string? email = HttpContext.Session.GetString("Email");
            int? id = HttpContext.Session.GetInt32("id");
            if (id.HasValue)
            {

                User? user = _patientRequest.GetUserById((int)id);
                IQueryable<PatientDashboard> result = from req in _context.Requests
                                                      join reqFile in _context.Requestwisefiles
                                                      on req.Requestid equals reqFile.Requestid into reqFilesGroup
                                                      where req.Userid == id
                                                      from reqFile in reqFilesGroup.DefaultIfEmpty()
                                                      select new PatientDashboard
                                                      {
                                                          User = user,
                                                          Request = req,
                                                          Requestwisefile = reqFile
                                                      };

                ViewData["Name"] = user?.Firstname;
                ViewBag.name = user?.Lastname;

                return View(result.ToList());
            }
            else
            {
                TempData["Error"] = "Something Went Wrong";
                return RedirectToAction("Index", "Login");
            }
        }
        #endregion

        #region AddRequestByMe
        public async Task<IActionResult> AddRequestByme()
        {
            int? id = HttpContext.Session.GetInt32("id");
            User? request = _patientRequest.GetUserById((int)id);
            RequestModel patientrequest = new RequestModel();
            patientrequest.Firstname = request?.Firstname ?? "";
            patientrequest.Lastname = request?.Lastname ?? "";
            patientrequest.Email = request?.Email ?? "";
            patientrequest.Street = request?.Street ?? "";
            patientrequest.City = request?.City ?? "";
            patientrequest.BirthDate = request != null && request.Intyear != null && request.Strmonth != null && request.Intdate != null
            ? new DateOnly((int)request.Intyear, int.Parse(request.Strmonth), (int)request.Intdate)
            : new DateOnly();
            patientrequest.Regions = _admin.GetAllRegion();
            patientrequest.State = request?.State ?? "";
            return View(patientrequest);
        }
        #endregion

        #region AddRequestByMePost
        [HttpPost]
        public async Task<IActionResult> AddRequestByme(RequestModel rm)
        {
            Blockrequest? block = _admin.GetBlockRequestByEmail(rm.Email);
            if (block == null)
            {

                var id = HttpContext.Session.GetInt32("id");
                if (rm.File != null)
                {
                    string uniqueFileName = await _patientRequest.AddFileInUploader(rm.File);
                    _patientRequest.AddPatientRequest(rm, ReqTypeId: 1);
                    Request? request = _patientRequest.GetRequestByEmail(rm.Email);
                    _patientRequest.AddRequestWiseFile(uniqueFileName, request?.Requestid ?? 1);
                    TempData["SuccessMessage"] = "Your Request Is Send Successful";
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _patientRequest.AddPatientRequest(rm, ReqTypeId: 1);
                    TempData["SuccessMessage"] = "Your Request Is Send Successful";
                    return RedirectToAction("Index", "Dashboard");
                }

            }
            else
            {
                TempData["Error"] = "Your Requests Is Block Please";
                return RedirectToAction("AddRequestByme", "Dashboard");
            }

        }
        #endregion

        #region AddRequestByElse
        public IActionResult AddRequestByElse()
        {
            List<Region> region = _admin.GetAllRegion();
            RequestModel requestmodal = new RequestModel();
            requestmodal.Regions = region;
            return View(requestmodal);
        }
        #endregion

        #region AddRequestByElse
        [HttpPost]
        public async Task<IActionResult> AddRequestByElse(RequestModel requestModel)
        {
            Blockrequest? block = _admin.GetBlockRequestByEmail(requestModel.Email);
            if (block == null)
            {

                int? id = HttpContext.Session.GetInt32("id");
                User? user = _patientRequest.GetUserById((int)id);
                if (requestModel.File != null)
                {
                    Region? region = _admin.GetRegionByName(requestModel.State);
                    string uniqueFileName = await _patientRequest.AddFileInUploader(requestModel.File);
                    Request request = new Request();
                    {
                        request.Userid = user?.Userid;
                        request.Firstname = user?.Firstname ?? "";
                        request.Lastname = user?.Lastname ?? "";
                        request.Email = user?.Email ?? "";
                        request.Requesttypeid = 2;
                        request.Createddate = DateTime.Now;
                        request.Status = 1;
                        request.Phonenumber = user?.Mobile ?? "";
                        request.Isdeleted = false;
                        request.Relationname = requestModel.Relation;
                        _context.Requests.Add(request);
                        _context.SaveChanges();

                    }

                    Requestclient requestclient = new Requestclient();
                    {
                        requestclient.Firstname = requestModel.Firstname;
                        requestclient.Requestid = request.Requestid;
                        requestclient.Lastname = requestModel.Lastname;
                        requestclient.Phonenumber = requestModel.PhoneNo;
                        requestclient.Email = requestModel.Email;
                        requestclient.State = requestModel.State;
                        requestclient.Street = requestModel.Street;
                        requestclient.Regionid = region?.Regionid;
                        requestclient.Zipcode = requestModel.Zipcode;
                        requestclient.Intdate = requestModel.BirthDate.Day;
                        requestclient.Intyear = requestModel.BirthDate.Year;
                        
                        requestclient.Strmonth = requestModel.BirthDate.Month.ToString();
                        _context.Requestclients.Add(requestclient);
                        _context.SaveChanges();
                    }
                    int count = _patientRequest.GetRequestCountByDate(request.Createddate);
                    if (region != null)
                    {
                        var confirmNum = string.Concat(region?.Abbreviation?.ToUpper(), request.Createddate.ToString("ddMMyy"), requestModel.Lastname.Substring(0, 2).ToUpper() ?? "",
                       requestModel.Firstname.Substring(0, 2).ToUpper(), count.ToString("D4"));
                        request.Confirmationnumber = confirmNum;
                    }
                    else
                    {
                        var confirmNum = string.Concat("ML", request.Createddate.ToString("ddMMyy"), requestModel.Lastname.Substring(0, 2).ToUpper() ?? "",
                      requestModel.Firstname.Substring(0, 2).ToUpper(), count.ToString("D4"));
                        request.Confirmationnumber = confirmNum;
                    }
                    _context.Requests.Update(request);
                    _context.SaveChanges();
                    _patientRequest.AddRequestWiseFile(uniqueFileName, request.Requestid);
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    var request = new Request();
                    {
                        request.Userid = user?.Userid;
                        request.Firstname = user?.Firstname ?? "";
                        request.Lastname = user?.Lastname ?? "";
                        request.Email = user?.Email ?? "";
                        request.Requesttypeid = 2;
                        request.Createddate = DateTime.Now;
                        request.Status = 1;
                        request.Phonenumber = user?.Mobile ?? "";
                        request.Isdeleted = false;
                        _context.Requests.Add(request);
                        _context.SaveChanges();

                    }
                    var requestclient = new Requestclient();
                    {
                        requestclient.Firstname = requestModel.Firstname;
                        requestclient.Requestid = request.Requestid;
                        requestclient.Lastname = requestModel.Lastname;
                        requestclient.Phonenumber = requestModel.PhoneNo;
                        requestclient.Email = requestModel.Email;
                        requestclient.State = requestModel.State;
                        requestclient.Street = requestModel.Street;
                        requestclient.Zipcode = requestModel.Zipcode;
                        requestclient.City = requestModel.City;
                        requestclient.Intdate = requestModel.BirthDate.Day;
                        requestclient.Intyear = requestModel.BirthDate.Year;
                        requestclient.Strmonth = requestModel.BirthDate.Month.ToString();
                        _context.Requestclients.Add(requestclient);
                        _context.SaveChanges();
                    }
                    return RedirectToAction("Index", "Dashboard");


                }

            }
            else
            {
                TempData["Error"] = "Your Requests Is Block Please";
                return RedirectToAction("AddRequestByElse", "Dashboard");
            }

        }
        #endregion
    }

}
