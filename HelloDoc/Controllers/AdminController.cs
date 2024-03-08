using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Plugins;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace HelloDoc.Controllers
{
    [CustomAuthorize("admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdmin _admin;
        private readonly IPatientRequest _patient;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IJwtAuth _jwtAuth;
        private readonly ILogin _login;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public AdminController(ApplicationDbContext context,IAdmin admin, IPatientRequest patient, IDataProtectionProvider dataProtectionProvider , IWebHostEnvironment hostingEnvironment,IJwtAuth jwtAuth,ILogin login)
        {
            _context = context;
            _admin = admin;
            _patient = patient;
            _jwtAuth = jwtAuth;
            _hostingEnvironment = hostingEnvironment;
            _login = login;
            _dataProtectionProvider = dataProtectionProvider;   
        }
        //main View
        public IActionResult ProviderLocation()
        {
            return View();
        }

        //main View
        public IActionResult Profile()
        {
            return View();
        }
        //main View
        public IActionResult Provider()
        {
            return View();  
        }
        //main View
        public IActionResult Parteners()
        {
            return View();
        }
        //main View
        public IActionResult Records()
        {
            return View();
        }
        //main View
        
        public IActionResult Index()
        {

            var request =  _admin.GetAllData();
            var newcount = request.Count(item => item.Status == 1);

            var pandingcount = request.Count(item => item.Status == 2);
            var activecount=request.Count(item=>item.Status==4 || item.Status==5);
            var conclude=request.Count(item => item.Status == 6);
            var toclosed=request.Count(item => item.Status == 3 || item.Status == 7 || item.Status == 8);
            var unpaid=request.Count(item => item.Status == 9);
            ViewBag.PandingCount = pandingcount;
            ViewBag.NewCount=newcount;
            ViewBag.activecount = activecount;
            ViewBag.conclude = conclude;
            ViewBag.toclosed = toclosed;
            ViewBag.unpaid = unpaid;
            return View(request.ToList());
        }   

        
        public IActionResult SearchPatient(string searchValue,string selectValue,string partialName,string selectedFilter, int[] currentStatus,int page,int pageSize=5)
            {
            var filteredPatients = _admin.SearchPatients(searchValue, selectValue, selectedFilter, currentStatus);
            int totalItems = filteredPatients.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedData = filteredPatients.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Create a ViewModel or use ViewBag/ViewData to store pagination information
            var newcount = filteredPatients.Count(item => item.Status == 1);

            var pandingcount = filteredPatients.Count(item => item.Status == 2);
            var activecount = filteredPatients.Count(item => item.Status == 4 || item.Status == 5);
            var conclude = filteredPatients.Count(item => item.Status == 6);
            var toclosed = filteredPatients.Count(item => item.Status == 3 || item.Status == 7 || item.Status == 8);
            var unpaid = filteredPatients.Count(item => item.Status == 9);
            ViewBag.PandingCount = pandingcount;
            ViewBag.NewCount = newcount;
            ViewBag.activecount = activecount;
            ViewBag.conclude = conclude;
            ViewBag.toclosed = toclosed;
            ViewBag.unpaid = unpaid; ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            string sessionData = HttpContext.Session.GetString("token");

            // Pass session data to the view
            ViewBag.SessionData = sessionData;


            return PartialView(partialName, paginatedData);
        }

        public IActionResult Learning()
        {
            return View();
        }


       
        
        public IActionResult PendingTablePartial()
        {
            return PartialView("PendingTablePartial");
        }
        public IActionResult ViewCase(int id)
        {
                var ViewCase = _admin.GetCaseById(id);

            return View(ViewCase);
        }

        [HttpPost]
        public async Task<IActionResult> ViewCase(ViewCaseVM viewCaseVM,int id)
        {
            var requestclient = await _context.Requestclients.FindAsync(id);
            if (ModelState.IsValid)
            {
                await _admin.UpdateRequestClient(viewCaseVM, id);
                return RedirectToAction("ViewCase", new { id });
            }
            return View();  
        }
        public IActionResult Viewnotes(int requestid)
        {
            ViewData["ViewName"] = "Dashboard";
            ViewData["RequestId"] = requestid;

            var result = _admin.GetNotesForRequest(requestid);

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRequest( int regionid, int physician, string description, int requestid,int status)
        {
            if (status == 1)
            {
            var result = await _admin.AssignRequest(regionid, physician, description, requestid);
            return Json(result);

            }
            if (status == 2)
            {
                var request = _context.Requests.FirstOrDefault(item => item.Requestid == requestid);
                request.Status = (short)status;
                request.Physicianid= physician;
                _context.Requests.Update(request);
                

                
               var requeststatuslog = new Requeststatuslog();
                requeststatuslog.Status = 2;
                requeststatuslog.Requestid=requestid;
                requeststatuslog.Notes = description;
                requeststatuslog.Createddate= DateTime.Now;
                requeststatuslog.Transtophysicianid = physician;
                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.SaveChanges();
                return Json(true);
            }
            return Json(false); 
        }

        [HttpPost]
        public async Task<IActionResult> ViewNotesPost(string adminNotes, int id)
        {
            if (adminNotes != null)
            {
                await _admin.UpdateAdminNotes(id, adminNotes);
            }

            return RedirectToAction("ViewNotes", new { requestid = id });
        }

        [HttpPost]
        public async  Task<IActionResult> CancelCase(string notes,string cancelReason, int requestid)
            {
            var result = await _admin.CancelCase(requestid, notes, cancelReason);
            return Json(result);

        }
        public IActionResult GetStatusCounts(int id)
        {
            var counts = new
            {
                NewCount = _context.Requests.Where(item=>item.Status==1).Count(),
                PendingCount = _context.Requests.Where(item=>item.Status==2).Count(),
                ActiveCount=_context.Requests.Where(item => item.Status == 4 || item.Status == 5).Count(),
                ToClosedCount = _context.Requests.Where(item => item.Status == 3 || item.Status == 7 ||item.Status==8).Count(),
                ConcludeCount=_context.Requests.Where(item => item.Status == 6).Count(),
                UnpaidCount = _context.Requests.Where(item => item.Status == 9).Count(),


            };
            return Json(counts);
        }

        public IActionResult GetPhysician(string region)
        {
            var physician = _context.Physicians.Where(p => p.Regionid == int.Parse(region)).ToList();
            return Ok(physician);
        }
        [HttpPost]
        public IActionResult BlockRequest(string blockreason,int requestid)
        {
            var request=_context.Requests.Find(requestid);
            request.Status = 11;
            _context.Requests.Update(request);  
            var block=new Blockrequest();
            block.Email = request.Email;
            block.Phonenumber=request.Phonenumber;
            block.Requestid =requestid.ToString();
            block.Createddate= DateTime.Now;    
            block.Reason= blockreason;
            _context.Blockrequests.Add(block);  
            var requeststatuslog=new Requeststatuslog();    
            requeststatuslog.Status = 11;
            requeststatuslog.Createddate= DateTime.Now;
            requeststatuslog.Requestid = requestid;
            requeststatuslog.Notes = blockreason;
            _context.Requeststatuslogs.Add(requeststatuslog);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ViewUploads(int id)
        {
            var reqfile = await _patient.GetRequestwisefileByIdAsync(id);
            var reqfiledeleted=reqfile.Where(item=>item.Isdeleted.Length== 0 || !item.Isdeleted[0]).ToList();
            var requestclient = _context.Requests.Where(item=>item.Requestid==id).FirstOrDefault(); 
            var requestwiseviewmodel = new RequestFileViewModel
            {
                Request = requestclient,
                Requestid = id,
                Requestwisefileview = reqfiledeleted
            };
            return View(requestwiseviewmodel);
        }
       
        [HttpPost]
        public IActionResult DeleteFile(string filename)
        {

            try
            {
                var file = _context.Requestwisefiles.FirstOrDefault(item => item.Filename == filename);
                    file.Isdeleted = new BitArray(new[] { true });
                    _context.Requestwisefiles.Update(file);
                    _context.SaveChanges();
                    return Ok(new { message = "File deleted successfully" ,id=file.Requestid});
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting file: {ex.Message}" });
            }
        }
        [HttpPost]
        public IActionResult DeleteSelectedFiles(List<string> filenames)
        {
            var url = Request.GetDisplayUrl;
            try
            {
                foreach (var filename in filenames)
                {
                    var file = _context.Requestwisefiles.FirstOrDefault(item => item.Filename == filename);

                    if (file != null)
                    {

                    
                            file.Isdeleted = new BitArray(new[] { true });
                            _context.SaveChanges();
                    }
                    }

                return Ok(new { message = "Files deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting files: {ex.Message}" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(RequestFileViewModel rm, int id)
        {
            if (rm.File != null)
            {
                var uniqueFileName = await _patient.AddFileInUploader(rm.File);
                _patient.AddRequestWiseFile(uniqueFileName, id);
                return RedirectToAction("ViewUploads", "Admin", new { id = id });
            }
            else
            {
                return RedirectToAction("ViewUploads", "Admin", new { id = id });

            }
        }

        [HttpPost]
        public IActionResult SendEmailWithSelectedFiles(List<string> filenames)
        {
            try
            {
                // Ensure filenames are not null or empty before proceeding
                if (filenames == null || filenames.Count == 0)
                {
                    return BadRequest(new { message = "No files selected to send in the email." });
                }

                // Get recipient email, subject, and body from your ViewModel or wherever it's stored
                string toEmail = "munavvarpopatiya999@gmail.com"; // Replace with your recipient email
                string subject = "Selected Files";
                string body = "Please find attached files.";

                // Call your repository method to send email
                bool isEmailSent = _admin.IsSendEmail(toEmail, subject, body, filenames);

                if (isEmailSent)
                {
                    return Ok(new { message = "Email sent successfully" });
                }
                else
                {
                    return StatusCode(500, new { message = "Error sending email" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error sending email: {ex.Message}" });
            }
        }

        public JsonResult CheckSession()
        {
            string sessionData = HttpContext.Session.GetString("token");

        
            if (string.IsNullOrEmpty(sessionData))
            {
                return Json(new { sessionExists = false });
            }
            else
            {
                return Json(new { sessionExists = true });
            }
        }
        public IActionResult SendOrder(int requestid)
        {
            var profession=_context.Healthprofessionaltypes.ToList();
            var business = _context.Healthprofessionals.ToList();
            var sendorder = new SendOrderModel();
            sendorder.requestid=requestid;
            sendorder.Healthprofessionaltypes=profession;
            sendorder.helthProfessional=business;
            return View(sendorder);
        }
        public IActionResult VendorNameByHelthProfession(int helthprofessionaltype)
        {
            var vendorname=_context.Healthprofessionals.Where(item=>item.Healthprofessionalid==helthprofessionaltype).ToList();
            return Ok(vendorname);
        }

        public IActionResult BusinessDetails(int vendorname)
        {
            var businessdetails=_context.Healthprofessionals.Where(item=>item.Healthprofessionalid == vendorname).FirstOrDefault();
            return Ok(businessdetails);
        }
        [HttpPost]
        public IActionResult SendOrder(int requestid,int business,string contact,string Email,string FaxNumber,string Prescription)
        {
            var orderdetail = new Orderdetail();
            orderdetail.Requestid=requestid;
            orderdetail.Email = Email;
            orderdetail.Prescription=Prescription;
            orderdetail.Vendorid = business;
            orderdetail.Faxnumber = FaxNumber;
            orderdetail.Businesscontact = contact;
            _context.Orderdetails.Add(orderdetail);  
            _context.SaveChanges();
            return Ok(new { message = "Order successfully", id = requestid });
        }
        [HttpPost]
        public IActionResult SendAgreement(int requestid, string agreementemail, string agreementphoneno)
        {
            var token = _jwtAuth.GenerateToken(agreementemail, agreementphoneno);
            var protector = _dataProtectionProvider.CreateProtector("munavvar");
            string requestidto=protector.Protect(requestid.ToString());


            var Agreemnet = Url.Action("ReviewAgreement", "Request", new { requestid = requestidto }, protocol: HttpContext.Request.Scheme);
            
            
            if (_login.IsSendEmail("tatva.dotnet.munavvarpopatiya@outlook.com", "Munavvar", $"Click <a href='{Agreemnet}'>here</a> to reset your password.")) {
                return Ok(new {Message="send a mail",id=requestid});
            
            }
            return Json(false);
        }

        public IActionResult agree(int id)
        {
            var request = _context.Requests.Where(item => item.Requestid == id).FirstOrDefault();
            request.Status = 4;
            _context.Update(request);
            _context.SaveChanges();
            return RedirectToAction("Index", "Login");
        }
    }
}
