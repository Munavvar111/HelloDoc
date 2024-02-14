using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                         where req.Email == email
                         from reqFile in reqFilesGroup.DefaultIfEmpty()
                         select new PatientDashboard
                         {
                             Request = req,
                             Requestwisefile = reqFile
                         };

            ViewBag.Email = email;
            ViewBag.name = user.Lastname;

            return View(result.Distinct().ToList());
        }


    }

}
