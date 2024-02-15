using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace HalloDocPatient.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _patientRequest;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DashboardController(ApplicationDbContext context, IPatientRequest patientRequest, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _patientRequest = patientRequest;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> ViewDocument(int requestid)
        {
            var reqfile = await _patientRequest.GetRequestwisefileByIdAsync(requestid);
            var viewmodel = new RequestFileViewModel
            {
                Requestid = requestid,
                Requestwisefileview = reqfile
            };
            return View(viewmodel);
        }
        public async Task<IActionResult> Profile()
        {
            var id = HttpContext.Session.GetInt32("id");
            var User = await _context.Users.FindAsync(id);
            DateTime BirthDate = new DateTime((int)User.Intyear, int.Parse(User.Strmonth), (int)User.Intdate);
            ViewBag.BirthDate = BirthDate.ToString("yyyy-MM-dd");
            var profildata = new ProfileVM();
            profildata.Email = User.Email;
            profildata.BirthDate = BirthDate;
            profildata.FirstName = User.Firstname;
            profildata.State = User.Lastname;
            profildata.City = User.City;
            profildata.Street = User.Street;
            profildata.ZipCode = User.Zipcode;
            profildata.LastName = User.Lastname;
            return View(profildata);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            var id = HttpContext.Session.GetInt32("id");
            var User = await _context.Users.FindAsync(id);


            if (ModelState.IsValid)
            {
                User.Firstname = profileVM.FirstName;
                User.Lastname = profileVM.LastName;
                User.Street = profileVM.Street; 
                User.City=profileVM.City;
                User.State= profileVM.State;
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
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + rm.File.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await rm.File.CopyToAsync(stream);
                }
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

            ViewBag.Email = email;
            ViewBag.name = user.Lastname;

            return View(result.Distinct().ToList());
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
            
            return View(patientrequest);
        }
        [HttpPost]
        public async Task<IActionResult> AddRequestByme(RequestModel rm)
        {
            var id = HttpContext.Session.GetInt32("id");

            if (rm.File != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + rm.File.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await rm.File.CopyToAsync(stream);
                }
                _patientRequest.AddPatientRequest(rm, ReqTypeId:1);
                var request = _patientRequest.GetRequestByEmail(rm.Email);
                _patientRequest.AddRequestWiseFile(uniqueFileName, request.Requestid);
                return RedirectToAction("Index","Dashboard");
            }
            return View();
        }

        public IActionResult AddRequestByElse()
        {
            return View();
        }
    }

}
