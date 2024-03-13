using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace HalloDocPatient.Controllers
{
    [CustomAuthorize("user")]
    public class DashboardController : Controller
    {
            private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _patientRequest;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DashboardController(ApplicationDbContext context, IPatientRequest patientRequest,IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _patientRequest = patientRequest;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> ViewDocument(int requestid)
        {
            var reqfile = await _patientRequest.GetRequestwisefileByIdAsync(requestid);
            var reqfiledeleted = reqfile.Where(item => item.Isdeleted.Length == 0 || !item.Isdeleted[0]).ToList();

            var id = HttpContext.Session.GetInt32("id");
            var user = _patientRequest.GetUserById((int)id);
            ViewData["Name"] = user.Firstname;
            var viewmodel = new RequestFileViewModel
            {
                User = user,
                Requestid = requestid,
                Requestwisefileview = reqfiledeleted
            };
            return View(viewmodel);
        }
        public async Task<IActionResult> Profile()
        {
            var id = HttpContext.Session.GetInt32("id");
            var User = await _context.Users.FindAsync(id);  
            DateTime BirthDate = new DateTime((int)User.Intyear, int.Parse(User.Strmonth), (int)User.Intdate);
            var regions = _context.Regions.ToList();
            ViewBag.BirthDate = BirthDate.ToString("yyyy-MM-dd");
            var profildata = new ProfileVM();
            profildata.Email = User.Email;
            profildata.BirthDate = BirthDate;
            profildata.PhoneNO = User.Mobile;
            profildata.FirstName = User.Firstname;
            profildata.State = User.State;
            profildata.City = User.City;
            profildata.Street = User.Street;
            profildata.ZipCode = User.Zipcode;
            profildata.LastName = User.Lastname;
            profildata.Regions=regions;
            ViewBag.State = User.State;
            ViewBag.City = User.City;
            ViewBag.Street = User.Street;

            return View(profildata);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            var id = HttpContext.Session.GetInt32("id");
            var User = await _context.Users.FindAsync(id);
            ViewData["Name"] = User.Firstname;
            
            if (ModelState.IsValid)
            {
                User.Firstname = profileVM.FirstName;
                User.Lastname = profileVM.LastName;
                User.Street = profileVM.Street; 
                User.City=profileVM.City;
                User.State= profileVM.State;
                User.Mobile = profileVM.PhoneNO;
                User.Zipcode = profileVM.ZipCode;
                User.Intdate =  profileVM.BirthDate.Day;
                User.Intyear = profileVM.BirthDate.Year;
                User.Strmonth = profileVM.BirthDate.Month.ToString();

                _context.Update(User);
                await _context.SaveChangesAsync();  
                return RedirectToAction("Profile");
            }
            else
            {
                return View(profileVM);  
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(RequestFileViewModel rm,int requestid) {
            var id = requestid;

            if (rm.File != null)
            {
                var uniqueFileName=await _patientRequest.AddFileInUploader(rm.File);
                _patientRequest.AddRequestWiseFile(uniqueFileName, requestid);
                return RedirectToAction("ViewDocument", "Dashboard", new { requestid = requestid });
            }
            else
            {
                return RedirectToAction("ViewDocument", "Dashboard", new { requestid = requestid });

            }
        }
        public IActionResult ViewFile(string filename)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot/uploads", filename);

            if (System.IO.File.Exists(filePath))
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

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
        public async Task<IActionResult> Index()
        {
            if (TempData["ShowToaster"] != null && (bool)TempData["ShowToaster"])
            {
                ViewBag.ShowToaster = true;
            }
            var email = HttpContext.Session.GetString("Email");
            var id =HttpContext.Session.GetInt32("id");
            var user = _patientRequest.GetUserById((int)id);
            var result = from req in _context.Requests
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

            ViewData["Name"] = user.Firstname;
            ViewBag.name = user.Lastname;

            return View(result.ToList());
        }

        public async Task<IActionResult> AddRequestByme()
        {
            var id = HttpContext.Session.GetInt32("id");
            var request = await _context.Users.FindAsync(id);
            var patientrequest =new RequestModel();
            patientrequest.Firstname = request.Firstname;
            patientrequest.Lastname = request.Lastname;
            patientrequest.Email = request.Email;
            patientrequest.Street = request.Street;
            patientrequest.City = request.City;
            patientrequest.BirthDate = new DateTime((int)request.Intyear, int.Parse(request.Strmonth), (int)request.Intdate);
            patientrequest.Regions = _context.Regions.ToList();
            patientrequest.State = request.State;
            return View(patientrequest);
        }
        [HttpPost]
        public async Task<IActionResult> AddRequestByme(RequestModel rm)
        {
            var block=_context.Blockrequests.Where(item=>item.Email == rm.Email).FirstOrDefault();
            if (block == null)
            {


                if (ModelState.IsValid)
                {


                    var id = HttpContext.Session.GetInt32("id");

                    if (rm.File != null)
                    {
                        var uniqueFileName = await _patientRequest.AddFileInUploader(rm.File);
                        _patientRequest.AddPatientRequest(rm, ReqTypeId: 1);
                        var request = _patientRequest.GetRequestByEmail(rm.Email);
                        _patientRequest.AddRequestWiseFile(uniqueFileName, request.Requestid);
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
            }
            else
            {
                TempData["Error"] = "Your Requests Is Block Please";
                return RedirectToAction("AddRequestByme","Dashboard");
            }

            return View(rm);
        }
        public IActionResult AddRequestByElse()
        {
            var region =_context.Regions.ToList();
            var requestmodal = new RequestModel();
            requestmodal.Regions=region;
            return View(requestmodal);
        }

        [HttpPost]
        public async Task<IActionResult> AddRequestByElse(RequestModel requestModel)
        {
            var block = _context.Blockrequests.Where(item => item.Email == requestModel.Email).FirstOrDefault();
            if(block == null) { 
            if (ModelState.IsValid)
            {

                var id = HttpContext.Session.GetInt32("id");
                var user = await _context.Users.FindAsync(id);
                if (requestModel.File != null)
                {
                    var uniqueFileName =await _patientRequest.AddFileInUploader(requestModel.File);
                    var request = new Request();
                    {
                        request.Userid = user.Userid;
                        request.Firstname = user.Firstname;
                        request.Lastname = user.Lastname;
                        request.Email = user.Email;
                        request.Requesttypeid = 2;
                        request.Createddate = DateTime.Now;
                        request.Status = 1;
                        request.Phonenumber = user.Mobile;
                        _context.Requests.Add(request);
                        _context.SaveChanges();

                    }
                    var requestclient = new Requestclient();
                    {
                        requestclient.Firstname = requestModel.Firstname;
                        requestclient.Requestid = request.Requestid;
                        requestclient.Lastname=requestModel.Lastname;
                        requestclient.Phonenumber = requestModel.PhoneNo;
                        requestclient.Email = requestModel.Email;
                        requestclient.State = requestModel.State;
                        requestclient.Street = requestModel.Street;
                            requestclient.Regionid=_context.Regions.Where(item=>item.Name==requestModel.State).FirstOrDefault().Regionid;
                        requestclient.Zipcode = requestModel.Zipcode;
                        requestclient.Intdate = requestModel.BirthDate.Day;
                        requestclient.Intyear = requestModel.BirthDate.Year;
                        requestclient.Strmonth = requestModel.BirthDate.Month.ToString();
                        _context.Requestclients.Add(requestclient);
                        _context.SaveChanges();
                    }
                    _patientRequest.AddRequestWiseFile(uniqueFileName, request.Requestid);
                    return RedirectToAction("Index","Dashboard");
                }
                    else
                    {
                        var request = new Request();
                        {
                            request.Userid = user.Userid;
                            request.Firstname = user.Firstname;
                            request.Lastname = user.Lastname;
                            request.Email = user.Email;
                            request.Requesttypeid = 2;
                            request.Createddate = DateTime.Now;
                            request.Status = 1;
                            request.Phonenumber = user.Mobile;
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
                    return View(requestModel);
                }
            }
            else
            {
                TempData["Error"] = "Your Requests Is Block Please";
                return RedirectToAction("AddRequestByElse", "Dashboard");
            }
            
        }
        
    }

}
