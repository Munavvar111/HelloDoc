using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Collections;
using BC = BCrypt.Net.BCrypt;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;

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

        public AdminController(ApplicationDbContext context, IAdmin admin, IPatientRequest patient, IDataProtectionProvider dataProtectionProvider, IWebHostEnvironment hostingEnvironment, IJwtAuth jwtAuth, ILogin login)
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

        public List<PhysicianLocation> GetProviders()
        {
            return _context.PhysicianLocations.ToList();
        }

        //main View
        public IActionResult Profile()
        {
            string? email = HttpContext.Session.GetString("Email");
            int? id = HttpContext.Session.GetInt32("id");

            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            AdminProfileVm adminProfile = new AdminProfileVm();

            if (admin != null)
            {
                adminProfile.FirstName = admin.Firstname;
                adminProfile.LastName = admin.Lastname ?? "";
                adminProfile.Email = admin.Email;
                adminProfile.Address1 = admin.Address1 ?? "";
                adminProfile.Address2 = admin.Address2 ?? "";
                adminProfile.City = admin.City ?? "";
                adminProfile.ZipCode = admin.Zip ?? "";
                adminProfile.MobileNo = admin.Mobile ?? "";
                adminProfile.Regions = _context.Regions.ToList();
                adminProfile.WorkingRegions = _context.AdminRegions.Where(item => item.Adminid == admin.Adminid).ToList();
                adminProfile.State = admin.Regionid;
            }
            else
            {
                TempData["Error"] = "Something Went Wrong Please Try Again";
                return RedirectToAction("Index", "Admin");
            }

            return View(adminProfile);
        }

        [HttpPost]
        public IActionResult ResetAdminPassword(string Password)
        {
            string? email = HttpContext.Session.GetString("Email");
            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            AspnetUser? account = _context.AspnetUsers.FirstOrDefault(item => item.Email == email);

            if (account != null && BC.Verify(Password, account.Passwordhash))
            {
                TempData["Error"] = "Your Previous Password Is Same As Current";
            }
            else if (account != null)
            {
                string passwordhash = BC.HashPassword(Password);
                account.Passwordhash = passwordhash;
                _context.AspnetUsers.Update(account);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Your Password Update SuccessFull";
            }
            else
            {
                TempData["Error"] = "Account not found";
            }

            return RedirectToAction("Profile", "Admin");
        }
        [HttpPost]
        public IActionResult AdministrationInfo(string email, string MobileNo, string[] adminRegion)
        {
            string? sessionemail = HttpContext.Session.GetString("Email");
            Admin? admin = _context.Admins.Where(item => item.Email == sessionemail).FirstOrDefault();
            if (admin != null)
            {
                admin.Email = email;
                admin.Mobile = MobileNo;
                _context.Admins.Update(admin); _context.SaveChanges();
                List<AdminRegion> existingregion = _context.AdminRegions.Where(item => item.Adminid == admin.Adminid).ToList();
                _context.AdminRegions.RemoveRange(existingregion);
                foreach (string regionValue in adminRegion)
                {
                    int regionId = int.Parse(regionValue); // Assuming region values are integer IDs
                    _context.AdminRegions.Add(new AdminRegion { Adminid = admin.Adminid, Regionid = regionId });
                }
                _context.SaveChanges(); // Save changes to add new associations
                TempData["SuccessMessage"] = "Your Administration Details Update SuccessFull";
            }
            else
            {
                TempData["Error"] = "Account not found";
            }
            return RedirectToAction("Profile", "Admin");

        }

        public IActionResult AccountingInfo(string Address1, string Address2, string City, int State, string Zipcode, string MobileNo)
        {
            string? sessionemail = HttpContext.Session.GetString("Email");
            Admin? admin = _context.Admins.Where(item => item.Email == sessionemail).FirstOrDefault();
            if (admin != null)
            {
                admin.Address1 = Address1;
                admin.Address2 = Address2;
                admin.City = City;
                admin.Zip = Zipcode;
                admin.Regionid = State;
                admin.Mobile = MobileNo;
                _context.Admins.Update(admin);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Your Accounting Details Update SuccessFull";
            }
            else
            {
                TempData["Error"] = "Account not found";
            }

            return RedirectToAction("Profile", "Admin");

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
            List<NewRequestTableVM> request = _admin.GetAllData();
            int newcount = request.Count(item => item.Status == 1);
            return View(request.ToList());
        }


        public IActionResult SearchPatient(string searchValue, string selectValue, string partialName, string selectedFilter, int[] currentStatus,bool exportdata,bool exportAllData, int page, int pageSize = 5)
        {
            List<NewRequestTableVM> filteredPatients = _admin.SearchPatients(searchValue, selectValue, selectedFilter, currentStatus);
            List<NewRequestTableVM> ExportData = _admin.SearchPatients(searchValue, selectValue, selectedFilter, currentStatus);
            int totalItems = filteredPatients.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<NewRequestTableVM> paginatedData = filteredPatients.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            if (exportdata)
            {
                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(writer,CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(filteredPatients);
                    writer.Flush();
                    var result = memoryStream.ToArray();
                    return File(result, "text/csv", "filtered_data.csv"); // Change content type and file name as needed
                }
            }
            if (exportAllData)
            {
                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(filteredPatients);
                    writer.Flush();
                    var result = memoryStream.ToArray();
                    return File(result, "text/csv", "filtered_data.csv"); // Change content type and file name as needed
                }
            }
            return PartialView(partialName, paginatedData);
        }
        public IActionResult ExportAll(string currentStatus)
        {
            var statusArray = currentStatus?.Split(',')?.Select(int.Parse).ToArray(); 
            var searchValue = "";
            var selectValue = "";
            var selectedFilter = "";
            List<NewRequestTableVM> filteredPatientsd = _admin.SearchPatients(searchValue, selectValue, selectedFilter, statusArray);
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(filteredPatientsd);
                writer.Flush();
                var result = memoryStream.ToArray();
                return File(result, "text/csv", "filtered_data.csv"); // Change content type and file name as needed
            }

            // Or return appropriate response format for export
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
            ViewCaseVM ViewCase = _admin.GetCaseById(id);
            return View(ViewCase);
        }

        [HttpPost]
        public async Task<IActionResult> ViewCase(ViewCaseVM viewCaseVM, int id)
        {
            Requestclient? requestclient = await _context.Requestclients.FindAsync(id);
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
            List<ViewNotesVM> result = _admin.GetNotesForRequest(requestid);
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRequest(int regionid, int physician, string description, int requestid, int status)
        {
            string? email = HttpContext.Session.GetString("Email");
            int? id = HttpContext.Session.GetInt32("id");
            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            if (admin != null)
            {

            bool result = await _admin.AssignRequest(regionid, physician, description, requestid,admin.Adminid);
                TempData["SuccessMessage"] = "Assign successfully";

                return Json(result);
            }
            else
            {
                return Json(false);
            }
        }

        public async Task<IActionResult> TransferRequest(int regionid, int physician, string description, int requestid, int status)
        {
            string? email = HttpContext.Session.GetString("Email");
            int? id = HttpContext.Session.GetInt32("id");
            Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
            if (admin != null)
            {

            bool result = await _admin.AssignRequest(regionid, physician, description, requestid,admin.Adminid);
            return Json(result);
            }
            else
            {
                return Json(false);
            }
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
        public async Task<IActionResult> CancelCase(string notes, string cancelReason, int requestid)
        {
            bool result = await _admin.CancelCase(requestid, notes, cancelReason);
            TempData["SuccessMessage"] = "Cancel successfully";
            return Json(result);

        }
        public IActionResult GetStatusCounts(int id)
        {
            var counts = new
            {
                NewCount = _context.Requests.Where(item => item.Status == 1).Count(),
                PendingCount = _context.Requests.Where(item => item.Status == 2).Count(),
                ActiveCount = _context.Requests.Where(item => item.Status == 4 || item.Status == 5).Count(),
                ToClosedCount = _context.Requests.Where(item => item.Status == 3 || item.Status == 7 || item.Status == 8).Count(),
                ConcludeCount = _context.Requests.Where(item => item.Status == 6).Count(),
                UnpaidCount = _context.Requests.Where(item => item.Status == 9).Count(),
            };
            return Json(counts);
        }


        public IActionResult GetPhysician(string region)
        {
            List<Physician> physician = _context.Physicians.Where(p => p.Regionid == int.Parse(region)).ToList();
            return Ok(physician);
        }
        [HttpPost]
        public IActionResult BlockRequest(string blockreason, int requestid)
        {
            bool success = _admin.BlockRequest(blockreason, requestid);
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
        #region ViewUploads
        public async Task<IActionResult> ViewUploads(int id)
        {
            List<Requestwisefile> reqfile = await _patient.GetRequestwisefileByIdAsync(id);
            List<Requestwisefile> reqfiledeleted = reqfile.Where(item => item.Isdeleted != null && (item.Isdeleted.Length == 0 || !item.Isdeleted[0])).ToList();
            Request? requestclient = _context.Requests.Where(item => item.Requestid == id).FirstOrDefault();
            RequestFileViewModel requestwiseviewmodel = new RequestFileViewModel
            {
                Request = requestclient,
                Requestid = id,
                Requestwisefileview = reqfiledeleted
            };
            return View(requestwiseviewmodel);
        }
        #endregion


        [HttpPost]
        public IActionResult DeleteFile(string filename)
        {
            try
            {
                Requestwisefile? file = _context.Requestwisefiles.FirstOrDefault(item => item.Filename == filename);
                if (file != null)
                {

                    file.Isdeleted = new BitArray(new[] { true });
                    _context.Requestwisefiles.Update(file);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Delete successful!";
                    return Ok(new { message = "File deleted successfully", id = file.Requestid });

                }
                else
                {
                    TempData["Error"] = "Delete Unsuccessful!";

                    return StatusCode(500, new { message = "File Is Not In Existes" });
                }
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
                foreach (string filename in filenames)
                {
                    Requestwisefile? file = _context.Requestwisefiles.FirstOrDefault(item => item.Filename == filename);

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
                String? uniqueFileName = await _patient.AddFileInUploader(rm.File);
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
            SendOrderModel sendOrderModel = _admin.GetSendOrder(requestid);

            return View(sendOrderModel);
        }
        public IActionResult VendorNameByHelthProfession(int helthprofessionaltype)
        {
            List<Healthprofessional> vendorname = _context.Healthprofessionals.Where(item => item.Healthprofessionalid == helthprofessionaltype).ToList();
            return Ok(vendorname);
        }

        public IActionResult BusinessDetails(int vendorname)
        {
            Healthprofessional? businessdetails = _context.Healthprofessionals.Where(item => item.Healthprofessionalid == vendorname).FirstOrDefault();
            return Ok(businessdetails);
        }

        [HttpPost]
        public IActionResult SendOrder(SendOrderModel order)
        {
            bool result = _admin.SendOrders(order);
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
            string requestidto = protector.Protect(requestid.ToString());
            string? Agreemnet = Url.Action("ReviewAgreement", "Request", new { requestid = requestidto }, protocol: HttpContext.Request.Scheme);
            if (_login.IsSendEmail("munavvarpopatiya777@gmail.com", "Munavvar", $"Click <a href='{Agreemnet}'>here</a> to reset your password."))
            {
                TempData["SuccessMessage"] = "Agreement Send successfully";
                return Ok(new { Message = "send a mail", id = requestid });
            }
            return Json(false);
        }
        [HttpPost]
        public IActionResult ClearCase(int requestidclearcase)
        {
            try
            {
                Request? request = _context.Requests.Where(item => item.Requestid == requestidclearcase).FirstOrDefault();
                if (request != null)
                {
                    request.Status = 10;
                    _context.Requests.Update(request);
                    _context.SaveChanges();

                    Requeststatuslog requeststatuslog = new Requeststatuslog();
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TempData["Error"] = "Something Went Wrong Please Try Again";
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public IActionResult GeneratePDF(int requestid)
        {

            ViewEncounterForm viewEncounterForm = _admin.GetEncounterForm(requestid);

            if (viewEncounterForm == null)
            {
                return NotFound();
            }

            //return View("EncounterFormDetails", encounterFormView);
            return new ViewAsPdf("EncounterFormDetails", viewEncounterForm)
            {
                FileName = "Encounter_Form.pdf"
            };

        }

        public IActionResult EncounterForm(int requestId)
        {
            ViewEncounterForm viewEncounterForm = new ViewEncounterForm();
            Encounterform? encounterFormByRequestId = _context.Encounterforms.FirstOrDefault(item => item.RequestId == requestId);

            if (encounterFormByRequestId != null && !encounterFormByRequestId.IsFinalize)
            {
                viewEncounterForm = _admin.GetEncounterForm(requestId);
                return View(viewEncounterForm);
            }
            else
            {
                Requestclient? request = _context.Requestclients.FirstOrDefault(item => item.Requestid == requestId);

                if (request != null)
                {
                    viewEncounterForm.FirstName = request.Firstname ?? "";
                    viewEncounterForm.LastName = request.Lastname ?? "";
                    viewEncounterForm.DateOfBirth = request.Intyear != null && request.Strmonth != null && request.Intdate != null
                    ? new DateOnly((int)request.Intyear, int.Parse(request.Strmonth), (int)request.Intdate)
                                                                            : new DateOnly();
                    viewEncounterForm.Email = request.Email ?? "";
                    viewEncounterForm.Location = $"{request.Address}, {request.City}, {request.State}";
                }
                else
                {
                    TempData["Error"] = "Something Went Wrong Please Try Again";
                    return RedirectToAction("Index", "Admin");
                }

                return View(viewEncounterForm);
            }
        }

        [HttpPost]
        public IActionResult EncounterForm(ViewEncounterForm viewEncounterForm, string requestid)
        {
            if (viewEncounterForm.IsFinalizied == "0")
            {
                _admin.SaveOrUpdateEncounterForm(viewEncounterForm, requestid);
                return RedirectToAction("EncounterForm", "Admin", new { requestid = requestid });
            }
            else
            {
                Encounterform? encounter = _context.Encounterforms.Where(item => item.RequestId == int.Parse(requestid)).FirstOrDefault();
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
                    Encounterform encounterFirsttime = new Encounterform();
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

        public IActionResult CloseCase(int requestid)
        {

            Requestclient? requestclient = _context.Requestclients.Where(item => item.Requestid == requestid).FirstOrDefault();
            List<Requestwisefile> requestwisedocument = _context.Requestwisefiles.Where(item => item.Requestid == requestid).ToList();
            Request? request = _context.Requests.Where(item => item.Requestid == requestid).FirstOrDefault();
            if (request != null)
            {

            string? requestclientnumber = request.Confirmationnumber;
            CloseCaseVM closecase = new CloseCaseVM();
            closecase.FirstName = requestclient?.Firstname ?? "";
            closecase.LastName = requestclient?.Lastname ?? "";
            closecase.Email = requestclient?.Email ?? "";
            closecase.PhoneNo = requestclient?.Phonenumber ?? "";
            closecase.BirthDate = requestclient !=null && requestclient.Intyear != null && requestclient.Strmonth != null && requestclient.Intdate != null
                                  ? new DateOnly((int)requestclient.Intyear, int.Parse(requestclient.Strmonth), (int)requestclient.Intdate)
                                  : new DateOnly();
            closecase.Requestwisefileview = requestwisedocument;
            closecase.ConfirmNumber = requestclientnumber;
            closecase.Requestid = requestid;
            return View(closecase);
            }
            else
            {
                TempData["Error"] = "Something Went Wrong Please Try Again";
                return RedirectToAction("Index","Admin");
            }
        }

        [HttpPost]
        public IActionResult CloseCase(CloseCaseVM closeCaseVM, int requestid)
        {
            if (!ModelState.IsValid)
            {
                return View(closeCaseVM);
            }
            else
            {
                Requestclient? requestclient = _context.Requestclients.Where(item => item.Requestid == requestid).FirstOrDefault();
                if(requestclient != null)
                {

                requestclient.Phonenumber = closeCaseVM.PhoneNo;
                requestclient.Email = closeCaseVM.Email;
                _context.Requestclients.Update(requestclient);
                }
                _context.SaveChanges();
                return RedirectToAction("CloseCase", new { requestid = requestid });
            }
        }

        public IActionResult CloseCaseModal(int requestid)
        {
            Request? request = _context.Requests.Where(item => item.Requestid == requestid).FirstOrDefault();
            if (request != null)
            {
                request.Status = 9;
            }
            Requeststatuslog requeststatuslog = new Requeststatuslog();
            requeststatuslog.Status = 9;
            requeststatuslog.Createddate = DateTime.Now;
            requeststatuslog.Requestid = requestid;
            _context.Requeststatuslogs.Add(requeststatuslog);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Close Case successfully";
            return RedirectToAction("Index", "Admin");
        }
    }
}
