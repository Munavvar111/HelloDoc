using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Plugins;
using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using Rotativa.AspNetCore;
using System;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Helpers;

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
            return View(request.ToList());
        }   

        
        public IActionResult SearchPatient(string searchValue,string selectValue,string partialName,string selectedFilter, int[] currentStatus,int page,int pageSize=3)
            {
            var filteredPatients = _admin.SearchPatients(searchValue, selectValue, selectedFilter, currentStatus);
            int totalItems = filteredPatients.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedData = filteredPatients.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages =totalPages;
            ViewBag.CurrentPage = page; ViewBag.PageSize = pageSize;

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
            
            var result = await _admin.AssignRequest(regionid, physician, description, requestid);
            return Json(result);

            
        }

        public async Task<IActionResult> TransferRequest(int regionid, int physician, string description, int requestid, int status)
        {
            var result = await _admin.AssignRequest(regionid, physician, description, requestid);
            return Json(result);
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
            var success = _admin.BlockRequest(blockreason, requestid);
            if (success)
            {
                TempData["SuccessMessage"] = "Block request successful!";
            }
            else
            {
                TempData["Error"] = "Block request Unsuccessful!";
            }
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
                TempData["SuccessMessage"] = "Delete successful!";

                return Ok(new { message = "File deleted successfully" ,id=file.Requestid});
               
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Delete Unsuccessful!";

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
                TempData["SuccessMessage"] = "Delete Selected successful!";

                return Ok(new { message = "Files deleted successfully" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Delete Selected Unsuccessful!";
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
                if (filenames == null || filenames.Count == 0)
                {
                    return BadRequest(new { message = "No files selected to send in the email." });
                }
                string toEmail = "munavvarpopatiya999@gmail.com"; 
                string subject = "Selected Files";
                string body = "Please find attached files.";

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

        
        public IActionResult SendOrder(int requestid)
        {
            var sendOrderModel = _admin.GetSendOrder(requestid);

            return View(sendOrderModel);
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
        public IActionResult SendOrder(SendOrderModel order)
        {
            


                var result = _admin.SendOrders(order);

                if (result)
                {
                    TempData["SuccessMessage"] = "Order successfully";
                }
                else
                {
                    TempData["Error"] = "Order Unsuccessfully";
                }
                return RedirectToAction("SendOrder", "Admin", new { requestid = order.requestid });

            
        
            
        }
        [HttpPost]
        public IActionResult SendAgreement(int requestid, string agreementemail, string agreementphoneno)
        {
            var protector = _dataProtectionProvider.CreateProtector("munavvar");
            string requestidto=protector.Protect(requestid.ToString());
            var Agreemnet = Url.Action("ReviewAgreement", "Request", new { requestid = requestidto }, protocol: HttpContext.Request.Scheme);
            if (_login.IsSendEmail("munavvarpopatiya777@gmail.com", "Munavvar", $"Click <a href='{Agreemnet}'>here</a> to reset your password.")) {
                TempData["SuccessMessage"] = "Agreement Send successfully";
                return Ok(new {Message="send a mail",id=requestid});
            }
            return Json(false);
        }
        [HttpPost]
        public IActionResult ClearCase(int requestidclearcase)
        {
            try
            {
                var request = _context.Requests.Where(item => item.Requestid == requestidclearcase).FirstOrDefault();
                if (request != null) {
                    request.Status = 10;
                    _context.Requests.Update(request);
                    _context.SaveChanges();

                    var requeststatuslog = new Requeststatuslog();
                    requeststatuslog.Status = 10;
                    requeststatuslog.Requestid = requestidclearcase;
                    requeststatuslog.Createddate = DateTime.Now;
                    _context.Requeststatuslogs.Add(requeststatuslog);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Clear Case successfully";
                   
                }
                else
                {
                    TempData["Error"] = "Clear Case Unsuccessfully";
                }
                return RedirectToAction("Index", "Admin");
            }
            catch(Exception ex) {
                TempData["Error"] = "Something Went Wrong Please Try Again";
                return RedirectToAction("Index", "Admin");

            }
        }

        [HttpGet]
        public IActionResult GeneratePDF(int requestid)
        {
            var encounterformbyrequestid = _context.Encounterforms.Where(item => item.RequestId == requestid).FirstOrDefault();
            var request = _context.Requestclients.Where(item => item.Requestid == requestid).FirstOrDefault();
            var viewencounterform = new ViewEncounterForm();

            viewencounterform.FirstName = request.Firstname;
            viewencounterform.LastName = request.Lastname;
            viewencounterform.DateOfBirth = new DateOnly((int)request.Intyear, int.Parse(request.Strmonth), (int)request.Intdate);
            viewencounterform.Email = request.Email;
            viewencounterform.Location = request.Address + ',' + request.City + ',' + request.State;
            viewencounterform.ABD = encounterformbyrequestid.Abd;

            viewencounterform.Skin = encounterformbyrequestid.Skin;
            viewencounterform.RR = encounterformbyrequestid.Rr;
            viewencounterform.Procedures = encounterformbyrequestid.Procedures;
            viewencounterform.CV = encounterformbyrequestid.Cv;
            viewencounterform.Chest = encounterformbyrequestid.Chest;
            viewencounterform.Allergies = encounterformbyrequestid.Allergies;
            viewencounterform.BPDiastolic = encounterformbyrequestid.BloodPressureDiastolic;
            viewencounterform.BPSystolic = encounterformbyrequestid.BloodPressureSystolic;
            viewencounterform.Diagnosis = encounterformbyrequestid.Diagnosis;
            viewencounterform.Followup = encounterformbyrequestid.FollowUp;
            viewencounterform.Heent = encounterformbyrequestid.Heent;
            viewencounterform.HistoryOfPresentIllness = encounterformbyrequestid.HistoryOfPresentIllnessOrInjury;
            viewencounterform.HR = encounterformbyrequestid.Hr;
            viewencounterform.MedicalHistory = encounterformbyrequestid.MedicalHistory;
            viewencounterform.Medications = encounterformbyrequestid.Medications;
            viewencounterform.MedicationsDispensed = encounterformbyrequestid.MedicationsDispensed;
            viewencounterform.Neuro = encounterformbyrequestid.Neuro;
            viewencounterform.O2 = encounterformbyrequestid.O2;
            viewencounterform.Other = encounterformbyrequestid.Other;
            viewencounterform.Pain = encounterformbyrequestid.Pain;
            viewencounterform.Procedures = encounterformbyrequestid.Procedures;
            viewencounterform.Temperature = encounterformbyrequestid.Temp;
            viewencounterform.TreatmentPlan = encounterformbyrequestid.TreatmentPlan;

            if (viewencounterform == null)
            {
                return NotFound();
            }
            //return View("EncounterFormDetails", encounterFormView);
            return new ViewAsPdf("EncounterFormDetails", viewencounterform)
            {
                FileName = "Encounter_Form.pdf"
            };

        }
        public IActionResult EncounterForm(int requestid)
        {
                var viewencounterform = new ViewEncounterForm();
            var encounterformbyrequestid = _context.Encounterforms.Where(item => item.RequestId == requestid).FirstOrDefault();
            if (encounterformbyrequestid !=null && !encounterformbyrequestid.IsFinalize)
            {
                var request = _context.Requestclients.Where(item => item.Requestid == requestid).FirstOrDefault();

                viewencounterform.FirstName = request.Firstname;
                viewencounterform.LastName = request.Lastname;
                viewencounterform.DateOfBirth = new DateOnly((int)request.Intyear, int.Parse(request.Strmonth), (int)request.Intdate);
                viewencounterform.Email = request.Email;
                viewencounterform.Location = request.Address + ',' + request.City + ',' + request.State;
                viewencounterform.ABD = encounterformbyrequestid.Abd;

                viewencounterform.Skin = encounterformbyrequestid.Skin;
                viewencounterform.RR = encounterformbyrequestid.Rr;
                viewencounterform.Procedures = encounterformbyrequestid.Procedures;
                viewencounterform.CV = encounterformbyrequestid.Cv;
                viewencounterform.Chest = encounterformbyrequestid.Chest;
                viewencounterform.Allergies = encounterformbyrequestid.Allergies;
                viewencounterform.BPDiastolic = encounterformbyrequestid.BloodPressureDiastolic;
                viewencounterform.BPSystolic = encounterformbyrequestid.BloodPressureSystolic;
                viewencounterform.Diagnosis = encounterformbyrequestid.Diagnosis;
                viewencounterform.Followup = encounterformbyrequestid.FollowUp;
                viewencounterform.Heent = encounterformbyrequestid.Heent;
                viewencounterform.HistoryOfPresentIllness = encounterformbyrequestid.HistoryOfPresentIllnessOrInjury;
                viewencounterform.HR = encounterformbyrequestid.Hr;
                viewencounterform.MedicalHistory = encounterformbyrequestid.MedicalHistory;
                viewencounterform.Medications = encounterformbyrequestid.Medications;
                viewencounterform.MedicationsDispensed = encounterformbyrequestid.MedicationsDispensed;
                viewencounterform.Neuro = encounterformbyrequestid.Neuro;
                viewencounterform.O2 = encounterformbyrequestid.O2;
                viewencounterform.Other = encounterformbyrequestid.Other;
                viewencounterform.Pain = encounterformbyrequestid.Pain;
                viewencounterform.Procedures = encounterformbyrequestid.Procedures;
                viewencounterform.Temperature = encounterformbyrequestid.Temp;
                viewencounterform.TreatmentPlan = encounterformbyrequestid.TreatmentPlan;
                return View(viewencounterform);

            }
            else
            {

            var request =_context.Requestclients.Where(item=>item.Requestid==requestid).FirstOrDefault();    
            
                viewencounterform.FirstName = request.Firstname;
                viewencounterform.LastName = request.Lastname;
                viewencounterform.DateOfBirth = new DateOnly((int)request.Intyear, int.Parse(request.Strmonth), (int)request.Intdate);
                viewencounterform.Email= request.Email;
                viewencounterform.Location = request.Address+','+request.City+','+request.State;
                
            return View(viewencounterform);
            }   
           
        }
        [HttpPost]
        public IActionResult EncounterForm(ViewEncounterForm viewEncounterForm,string requestid)
        {
            if (viewEncounterForm.IsFinalizied == "0")
            {
                var encounter = _context.Encounterforms.Where(item=>item.RequestId==int.Parse(requestid)).FirstOrDefault();
                if (encounter == null)
                {
                    var encounterFirsttime = new Encounterform();
                    encounterFirsttime.Abd = viewEncounterForm.ABD;
                    encounterFirsttime.Skin = viewEncounterForm.Skin;
                    encounterFirsttime.Rr = viewEncounterForm.RR;
                    encounterFirsttime.Procedures = viewEncounterForm.Procedures;
                    encounterFirsttime.Cv = viewEncounterForm.CV;
                    encounterFirsttime.Chest = viewEncounterForm.Chest;
                    encounterFirsttime.Allergies = viewEncounterForm.Allergies;
                    encounterFirsttime.BloodPressureDiastolic = viewEncounterForm.BPDiastolic;
                    encounterFirsttime.BloodPressureSystolic = viewEncounterForm.BPSystolic;
                    encounterFirsttime.Diagnosis = viewEncounterForm.Diagnosis;
                    encounterFirsttime.FollowUp = viewEncounterForm.Followup;
                    encounterFirsttime.RequestId = int.Parse(requestid);
                    encounterFirsttime.Heent = viewEncounterForm.Heent;
                    encounterFirsttime.HistoryOfPresentIllnessOrInjury = viewEncounterForm.HistoryOfPresentIllness;
                    encounterFirsttime.Hr = viewEncounterForm.HR;
                    encounterFirsttime.IsFinalize = false;
                    encounterFirsttime.MedicalHistory = viewEncounterForm.MedicalHistory;
                    encounterFirsttime.Medications = viewEncounterForm.Medications;
                    encounterFirsttime.MedicationsDispensed = viewEncounterForm.MedicationsDispensed;
                    encounterFirsttime.Neuro = viewEncounterForm.Neuro;
                    encounterFirsttime.O2 = viewEncounterForm.O2;
                    encounterFirsttime.Other = viewEncounterForm.Other;
                    encounterFirsttime.Pain = viewEncounterForm.Pain;
                    encounterFirsttime.Procedures = viewEncounterForm.Procedures;
                    encounterFirsttime.Temp = viewEncounterForm.Temperature;
                    encounterFirsttime.TreatmentPlan = viewEncounterForm.TreatmentPlan;
                    _context.Encounterforms.Add(encounterFirsttime);
                    _context.SaveChanges();
                }
                else
                {
                    
                encounter.Abd = viewEncounterForm.ABD;
                encounter.Skin = viewEncounterForm.Skin;
                encounter.Rr = viewEncounterForm.RR;
                encounter.Procedures = viewEncounterForm.Procedures;
                encounter.Cv = viewEncounterForm.CV;
                encounter.Chest=viewEncounterForm.Chest;
                encounter.Allergies= viewEncounterForm.Allergies;
                encounter.BloodPressureDiastolic = viewEncounterForm.BPDiastolic;
                encounter.BloodPressureSystolic=viewEncounterForm.BPSystolic;
                encounter.Diagnosis= viewEncounterForm.Diagnosis;
                encounter.FollowUp = viewEncounterForm.Followup;
                encounter.RequestId = int.Parse(requestid);
                encounter.Heent=viewEncounterForm.Heent;
                encounter.HistoryOfPresentIllnessOrInjury = viewEncounterForm.HistoryOfPresentIllness;
                encounter.Hr = viewEncounterForm.HR;
                encounter.IsFinalize = false;
                encounter.MedicalHistory = viewEncounterForm.MedicalHistory;
                encounter.Medications=viewEncounterForm.Medications;
                encounter.MedicationsDispensed = viewEncounterForm.MedicationsDispensed;
                encounter.Neuro = viewEncounterForm.Neuro;
                encounter.O2 = viewEncounterForm.O2;
                encounter.Other = viewEncounterForm.Other;
                encounter.Pain = viewEncounterForm.Pain;
                encounter.Procedures=viewEncounterForm.Procedures;
                encounter.Temp = viewEncounterForm.Temperature;
                encounter.TreatmentPlan = viewEncounterForm.TreatmentPlan;
                _context.Encounterforms.Update(encounter);    
                _context.SaveChanges();
                }
            return RedirectToAction("EncounterForm", "Admin", new { requestid =requestid});
            }
            else
            {
                var encounter = _context.Encounterforms.Where(item => item.RequestId == int.Parse(requestid)).FirstOrDefault();
                if (encounter != null)
                {

                encounter.Abd = viewEncounterForm.ABD;
                encounter.Skin = viewEncounterForm.Skin;
                encounter.RequestId = int.Parse(requestid);
                encounter.Rr = viewEncounterForm.RR;
                encounter.Procedures = viewEncounterForm.Procedures;
                encounter.Cv = viewEncounterForm.CV;
                encounter.Chest = viewEncounterForm.Chest;
                encounter.Allergies = viewEncounterForm.Allergies;
                encounter.BloodPressureDiastolic = viewEncounterForm.BPDiastolic;
                encounter.BloodPressureSystolic = viewEncounterForm.BPSystolic;
                encounter.Diagnosis = viewEncounterForm.Diagnosis;
                encounter.FollowUp = viewEncounterForm.Followup;
                encounter.Heent = viewEncounterForm.Heent;
                encounter.HistoryOfPresentIllnessOrInjury = viewEncounterForm.HistoryOfPresentIllness;
                encounter.Hr = viewEncounterForm.HR;
                encounter.IsFinalize = true;
                encounter.MedicalHistory = viewEncounterForm.MedicalHistory;
                encounter.Medications = viewEncounterForm.Medications;
                encounter.MedicationsDispensed = viewEncounterForm.MedicationsDispensed;
                encounter.Neuro = viewEncounterForm.Neuro;
                encounter.O2 = viewEncounterForm.O2;
                encounter.Other = viewEncounterForm.Other;
                encounter.Pain = viewEncounterForm.Pain;
                encounter.Procedures = viewEncounterForm.Procedures;
                encounter.Temp = viewEncounterForm.Temperature;
                encounter.TreatmentPlan = viewEncounterForm.TreatmentPlan;
                _context.Encounterforms.Update(encounter);
                _context.SaveChanges();
                }
                else
                {
                    var encounterFirsttime = new Encounterform();
                    encounterFirsttime.Abd = viewEncounterForm.ABD;
                    encounterFirsttime.Skin = viewEncounterForm.Skin;
                    encounterFirsttime.Rr = viewEncounterForm.RR;
                    encounterFirsttime.Procedures = viewEncounterForm.Procedures;
                    encounterFirsttime.Cv = viewEncounterForm.CV;
                    encounterFirsttime.Chest = viewEncounterForm.Chest;
                    encounterFirsttime.Allergies = viewEncounterForm.Allergies;
                    encounterFirsttime.BloodPressureDiastolic = viewEncounterForm.BPDiastolic;
                    encounterFirsttime.BloodPressureSystolic = viewEncounterForm.BPSystolic;
                    encounterFirsttime.Diagnosis = viewEncounterForm.Diagnosis;
                    encounterFirsttime.FollowUp = viewEncounterForm.Followup;
                    encounterFirsttime.RequestId = int.Parse(requestid);
                    encounterFirsttime.Heent = viewEncounterForm.Heent;
                    encounterFirsttime.HistoryOfPresentIllnessOrInjury = viewEncounterForm.HistoryOfPresentIllness;
                    encounterFirsttime.Hr = viewEncounterForm.HR;
                    encounterFirsttime.IsFinalize = true;
                    encounterFirsttime.MedicalHistory = viewEncounterForm.MedicalHistory;
                    encounterFirsttime.Medications = viewEncounterForm.Medications;
                    encounterFirsttime.MedicationsDispensed = viewEncounterForm.MedicationsDispensed;
                    encounterFirsttime.Neuro = viewEncounterForm.Neuro;
                    encounterFirsttime.O2 = viewEncounterForm.O2;
                    encounterFirsttime.Other = viewEncounterForm.Other;
                    encounterFirsttime.Pain = viewEncounterForm.Pain;
                    encounterFirsttime.Procedures = viewEncounterForm.Procedures;
                    encounterFirsttime.Temp = viewEncounterForm.Temperature;
                    encounterFirsttime.TreatmentPlan = viewEncounterForm.TreatmentPlan;
                    _context.Encounterforms.Add(encounterFirsttime);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index", "Admin");
            }
        }
    }
}
