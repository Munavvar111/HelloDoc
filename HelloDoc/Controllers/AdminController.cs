using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using CsvHelper;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using ServiceStack;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web.Helpers;
using static DataAccessLayer.CustomModel.InvoiceVM;
using BC = BCrypt.Net.BCrypt;


namespace HelloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdmin _admin;
        private readonly IPatientRequest _patient;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IInvoiceInterface _invoiceInterface;
        private readonly IJwtAuth _jwtAuth;
        private readonly ILogin _login;
        private readonly IUploadProvider _uploadProvider;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IEmailServices _emailService;

        public AdminController(IUploadProvider uploadProvider, IEmailServices emailServices,IInvoiceInterface invoiceInterface, IAdmin admin, IPatientRequest patient, IDataProtectionProvider dataProtectionProvider, IWebHostEnvironment hostingEnvironment, IJwtAuth jwtAuth, ILogin login)
        {
            _admin = admin;
            _uploadProvider = uploadProvider;
            _invoiceInterface = invoiceInterface;
            _patient = patient;
            _jwtAuth = jwtAuth;
            _emailService = emailServices;
            _hostingEnvironment = hostingEnvironment;
            _login = login;
            _dataProtectionProvider = dataProtectionProvider;
        }

        #region ProviderLocationPage
        [CustomAuthorize("ProviderLocation", "17")]
        //ProviderLocation View
        public IActionResult ProviderLocation()
        {
            return View();
        }

        //getAll PhysicianLocation
        [CustomAuthorize("ProviderLocation", "17")]
        public List<PhysicianLocation> GetProviders()
        {
            return _admin.GetAllPhysicianLocation();
        }
        #endregion

        #region AdminProfilePage

        #region Profile
        //Admin Profile Page

        [CustomAuthorize("MyProfile", "5")]
        public IActionResult Profile()
        {

            try
            {
                ///Get The Email From The Session

                string? email = HttpContext.Session.GetString("Email");
                //Check The Session Is Null Or Not
                if (string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "Invalid session data. Please log in again.";
                    return RedirectToAction("Index", "Login");
                }

                //GetProfile Date From Controller
                AdminProfileVm adminProfile = _admin.GetAdminProfile(email);

                //GetAll Role Regardign The Admin
                ViewBag.Roles = _admin.GetRoleFromAccountType(1);

                //Check adminProfile Is Not Null
                if (adminProfile == null)
                {
                    TempData["Error"] = "Something went wrong while fetching admin profile.";
                    return RedirectToAction("Index", "Admin");
                }

                return View(adminProfile);
            }
            catch (Exception ex)
            {

                TempData["Error"] = "An error occurred while processing your request. Please try again later." + ex.Message;
                return RedirectToAction("Index", "Admin");
            }
        }

        #endregion

        #region ProfileByID
        [CustomAuthorize("MyProfile", "5")]
        //Edit The Profile From UserAccess
        public IActionResult EditProfile(int adminid)
        {
            try
            {
                if (adminid == 0)
                {
                    TempData["Error"] = "Admin ID value is not valid.";
                    return RedirectToAction("Index", "Admin");
                }

                // Get Admin Details From The AdminID
                Admin? admin = _admin.GetAdminEmailById(adminid);
                if (admin == null)
                {
                    TempData["Error"] = "Admin details not found.";
                    return RedirectToAction("Index", "Admin");
                }

                // Get Particular Admin Data
                AdminProfileVm adminProfile = _admin.GetAdminProfile(admin.Email);
                ViewBag.IsEdit = true;
                if (adminProfile == null)
                {
                    TempData["Error"] = "Admin profile data not found.";
                    return RedirectToAction("Index", "Admin");
                }

                return View("Profile", adminProfile);
            }
            catch (Exception ex)
            {

                TempData["Error"] = "An error occurred while processing your request. Please try again later." + ex.Message;
                return RedirectToAction("Index", "Admin");
            }
        }
        #endregion

        #region ResetAdminPassword
        [CustomAuthorize("MyProfile", "5")]
        [HttpPost]
        public IActionResult ResetAdminPassword(string password)
        {
            string? email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Invalid session data. Please log in again.";
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                //Reset The Admin Profile
                _admin.ResetAdminPassword(email, password);
                TempData["SuccessMessage"] = "Your password has been updated successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Profile", "Admin");
        }
        #endregion

        #region AdministrationInfo
        [CustomAuthorize("MyProfile", "5")]
        [HttpPost]
        public IActionResult AdministrationInfo(string EmailProvider, string mobileNo, string[] adminRegion)
        {
            //Session Email Is Null Or Not Check
            string? sessionEmail = HttpContext.Session.GetString("Email");

            AspnetUser? AspnetUser = _patient.GetAspnetUserBYEmail(EmailProvider);
            //Check If  Session  Is Not Equal To The UpdatedEmail 
            if (sessionEmail != EmailProvider)
            {
                //Check Email Is Already Exist In AspNetUser
                if (AspnetUser != null)
                {
                    TempData["Error"] = "Email Already Exists Pleased Change the Email";
                    return RedirectToAction("Index", "Admin");
                }
            }
            //If Updated Email Is Null Or Ematy
            if (string.IsNullOrEmpty(sessionEmail))
            {
                TempData["Error"] = "Invalid session data. Please log in again.";
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                //update Admin Administration Details Like Email.PhoneNo,its Working Region
                _admin.UpdateAdministrationInfo(sessionEmail, EmailProvider, mobileNo, adminRegion);
                HttpContext.Session.SetString("Email", EmailProvider);
                TempData["SuccessMessage"] = "Your administration details have been updated successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Profile", "Admin");
        }
        #endregion

        #region AccountingInfo
        [CustomAuthorize("MyProfile", "5")]
        public IActionResult AccountingInfo(string address1, string address2, string city, int state, string zipcode, string mobileNo)
        {
            string? sessionEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(sessionEmail))
            {
                TempData["Error"] = "Invalid session data. Please log in again.";
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                //In Admin UpdateAccountingInfo Update Address ,City ,Zipcose ,State Details
                _admin.UpdateAccountingInfo(sessionEmail, address1, address2, city, zipcode, state, mobileNo);
                TempData["SuccessMessage"] = "Your accounting details have been updated successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Profile", "Admin");
        }
        #endregion

        #endregion

        #region ProviderDropDown

        #region ProviderPage

        #region Provider
        [CustomAuthorize("Provider", "8")]
        public IActionResult Provider()
        {
            //In Admin Dashboard Provider Page In This All Provider Details
            List<Region> region = _admin.GetAllRegion();
            return View(region);
        }
        #endregion

        #region ProviderData
        [CustomAuthorize("Provider", "8")]
        public IActionResult ProviderData(string region)
        {
            //Get All Provider Data By Its Working Region
            List<ProviderVM> providers = _admin.GetProviders(region);
            return PartialView("ProviderProfileTablePartial", providers);
        }
        #endregion

        #region ContactProvider
        public IActionResult ContactProvider(int ProviderId, int radioForprovider, string messageForProvider)
        {
            //GetPhysician Details Using PhysicianId
            Physician physician = _admin.GetPhysicianById(ProviderId);
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            //Get Admin Details Using Session Email
            Admin? admin = _admin.GetAdminByEmail(Email);

            if (radioForprovider == 1)
            {
                //Send The Mail For Physician
                string to = physician.Email;
                string subject = "Contact Provider";
                string body = "Please Connect Through To Use For Further Conversation";
                if (_login.IsSendEmail(to, subject, body))
                {
                    _emailService.EmailLog(to, body, subject, physician.Firstname + " " + physician.Lastname, admin.Roleid, 0, admin.Adminid, physician.Physicianid, 0, true, 1);
                    TempData["SuccessMessage"] = "Send Request successful!";
                    return RedirectToAction("Provider", "Admin");
                }
                else
                {
                    TempData["Error"] = "Send Request successful!";
                    return RedirectToAction("Provider", "Admin");

                }
            }
            return RedirectToAction("Provider", "Admin");
        }
        #endregion

        #region DeletePhysician
        public bool DeltePhysician(string Email)
        {
            //Delete The Physician In Edit Physician Page 
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            if (physician == null)
            {
                return false;
            }
            physician.Isdeleted = true;
            _admin.UpdatePhysicianDataBase(physician);
            _admin.SaveChanges();
            return true;
        }
        #endregion

        #region PhysicanProfile
        [CustomAuthorize("Provider", "8", "22")]
        [HttpGet("Admin/PhysicanProfile/{id}", Name = "AdminProviderProfile")]
        [HttpGet("Provider/PhysicanProfile", Name = "ProviderMyProfile")]
        public IActionResult PhysicanProfile(int id)
        {
            //Edit Physician Profile Page In that Admin Edit The Physician Details
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            if (physician != null)
            {
                id = physician.Physicianid;
            }
            try
            {
                if (admin != null)
                {
                    ViewBag.IsPhysician = false;
                }
                if (physician != null)
                {
                    ViewBag.IsPhysician = true;
                }
                ProviderProfileVm providerProfile = _admin.GetPhysicianProfile(id);
                return View(providerProfile);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                if (admin != null)
                    return RedirectToAction("Index", "Admin");
                else return RedirectToAction("Index", "Provider");
            }
        }
        #endregion


        #region ResetPhysicianPassword
        //Reset Physician Profile like its status
        [CustomAuthorize("Provider", "8", "22")]
        public IActionResult ResetPhysicianPassword(string password, int physicianId, string Username, int Status, int PhysicianRole)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Physician physician = _admin.GetPhysicianById(physicianId);
            Admin? admin = _admin.GetAdminByEmail(Email);
            if (physician == null || physician.Aspnetuser == null)
            {
                TempData["Error"] = "Something Went Wrong";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (password == null)
                {
                    //If Password Is NUll Then Update Its Username,role,status Detils
                    physician.Aspnetuser.Username = Username;
                    physician.Roleid = PhysicianRole;
                    physician.Status = (short?)Status;
                    physician.Modifiedby = admin != null ? admin.Aspnetuserid : physician.Aspnetuserid;
                    physician.Modifieddate = DateTime.Now;
                    _admin.UpdatePhysicianDataBase(physician);
                    _admin.SaveChanges();

                }
                else
                {
                    //if password is not null then update Its password details
                    bool passwordChanged = _admin.ResetPhysicianPassword(physicianId, password);
                    if (passwordChanged)
                    {
                        TempData["SuccessMessage"] = "Your password has been updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Your previous password is the same as the current one.";
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            if (admin != null)
                return RedirectToAction("PhysicanProfile", "Admin", new { id = physicianId });
            else
                return RedirectToAction("PhysicanProfile", "Provider", new { id = physicianId });

        }
        #endregion

        #region SaveSignatureImage
        [HttpPost]
        [CustomAuthorize("Provider", "8", "22")]

        public IActionResult SaveSignatureImage(IFormFile signatureImage, int id)
        {
            try
            {
                //check the signature if is not null or not
                if (signatureImage != null && signatureImage.Length > 0)
                {
                    string fileName = _uploadProvider.UploadSignature(signatureImage, id);
                    Physician physician = _admin.GetPhysicianById(id);
                    physician.Signature = fileName;
                    _admin.UpdatePhysicianDataBase(physician);
                    _admin.SaveChanges();
                    TempData["SuccessMessage"] = "Signature saved successfully.";
                    return Ok();
                }
                else
                {
                    TempData["Error"] = "Signature saved Unsuccessfully.";
                    return BadRequest("No signature image received.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving signature: {ex.Message}");
            }
        }
        #endregion

        #region UploadDoc
        [CustomAuthorize("Provider", "8", "22")]
        [HttpPost]
        public IActionResult UploadDoc(string fileName, IFormFile File, int physicianid)
        {
            //Upload The Onboarding Docs in the database
            Physician? physician = _admin.GetPhysicianById(physicianid);
            if (physician != null)
            {

                if (fileName == "ICA")
                {
                    string docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.Isagreementdoc = new BitArray(new[] { true });
                }
                if (fileName == "Background")
                {
                    string docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.Isbackgrounddoc = new BitArray(new[] { true });
                }
                if (fileName == "Hippa")
                {
                    string docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.Istrainingdoc = new BitArray(new[] { true });
                }
                if (fileName == "NonDiscoluser")
                {
                    string docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.Isnondisclosuredoc = new BitArray(new[] { true });
                }
                if (fileName == "License")
                {
                    string docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.Islicensedoc = new BitArray(new[] { true });
                }
                _admin.UpdatePhysicianDataBase(physician);
                _admin.SaveChanges();
                TempData["SuccessMessage"] = "Upload Doc successfully.";
                return Ok();
            }
            else
            {
                return BadRequest("No Doc File received.");
            }
        }
        #endregion

        #region PhysicianInformation
        [CustomAuthorize("Provider", "8", "22")]
        [HttpPost]
        public IActionResult PhysicianInformation(string EmailProvider, int id, string mobileNo, string[] adminRegion, string synchronizationEmail, string npinumber, string medicalLicense)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianById(id);
            Physician? physicianSession = _admin.GetPhysicianByEmail(Email);
            AspnetUser? aspnetUser = _patient.GetAspnetUserBYEmail(Email);

            if (physician.Email != EmailProvider)
            {

                if (aspnetUser != null)
                {
                    TempData["Error"] = "This Email Is Already Exists Please Change";
                    if (admin != null)
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "Provider");
                }
            }

            try
            {
                if (admin != null)
                    _admin.UpdatePhysicianInformation(id, EmailProvider, mobileNo, adminRegion, synchronizationEmail, npinumber, medicalLicense, admin.Aspnetuserid);
                else if (physicianSession != null)
                    _admin.UpdatePhysicianInformation(id, EmailProvider, mobileNo, adminRegion, synchronizationEmail, npinumber, medicalLicense, physician.Aspnetuserid);
                TempData["SuccessMessage"] = "Physician information has been updated successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            if (admin != null)
                return RedirectToAction("PhysicanProfile", "Admin", new { id = id });
            else
                return RedirectToAction("PhysicanProfile", "Provider", new { id = id });
        }
        #endregion

        #region Providerprofile
        [CustomAuthorize("Provider", "8", "22")]
        [HttpPost]
        public IActionResult Providerprofile(int id, string businessName, string businessWebsite, IFormFile signature, IFormFile photoFile)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);

            try
            {
                if (admin != null)
                    _admin.UpdateProviderProfile(id, businessName, businessWebsite, signature, photoFile, admin.Aspnetuserid);
                if (physician != null)
                    _admin.UpdateProviderProfile(id, businessName, businessWebsite, signature, photoFile, physician.Aspnetuserid);
                TempData["SuccessMessage"] = "Provider profile has been updated successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            if (admin != null)
                return RedirectToAction("PhysicanProfile", "Admin", new { id = id });
            else
                return RedirectToAction("PhysicanProfile", "Provider", new { id = id });
        }
        #endregion

        #region ProviderAccountingInfo
        [CustomAuthorize("Provider", "8", "22")]
        [HttpPost]
        public IActionResult ProviderAccountingInfo(int physicianid, string Address1, string Address2, string City, int State, string Zipcode, string MobileNo)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            bool success = false;


            if (admin != null)
            {

                success = _admin.UpdatePhysicianAccountingInfo(physicianid, Address1, Address2, City, State, Zipcode, MobileNo, admin.Aspnetuserid);
            }
            if (physician != null)
            {
                success = _admin.UpdatePhysicianAccountingInfo(physicianid, Address1, Address2, City, State, Zipcode, MobileNo, physician.Aspnetuserid);
            }

            if (success)
            {
                TempData["SuccessMessage"] = "Provider AccountingInfo Saved.";
            }
            else
            {
                TempData["ErrorMessage"] = "Provider not found or invalid ID.";
            }
            if (admin != null)
            {
                return RedirectToAction("PhysicanProfile", "Admin", new { id = physicianid });
            }
            else
            {
                return RedirectToAction("PhysicanProfile", "Provider", new { id = physicianid });
            }
        }
        #endregion

        #region CreateProvider
        public IActionResult CreateProvider()
        {
            List<Region> regions = _admin.GetAllRegion();
            List<Role> roles = _admin.GetRoleFromAccountType(2);
            List<Role> providerrole = roles.Where(item => !item.Isdeleted[0]).ToList();
            CreateProviderVM createProvider = new CreateProviderVM();
            createProvider.Regions = regions;
            createProvider.Roles = providerrole;
            return View(createProvider);
        }
        #endregion

        #region SaveNotification
        [HttpPost]
        public IActionResult SaveNotification(List<int> physicianIds, List<bool> checkboxStates)
        {
            _admin.SaveNotification(physicianIds, checkboxStates);
            return Ok();
        }
        #endregion

        #region CreateProviderPost
        [HttpPost]
        public IActionResult CreateProvider(CreateProviderVM createProvider, string[] adminRegion)
        {
            string? email = HttpContext.Session.GetString("Email");
            string? id = HttpContext.Session.GetString("aspnetid");
            if (email == null)
            {
                TempData["Error"] = "Session Is Expire Please Login!";
                return RedirectToAction("Index", "Login");
            }
            Admin? admin = _admin.GetAdminByEmail(email);

            AspnetUser aspnetUserExists = _admin.GetAspNetUserByEmail(createProvider.Email);
            if (aspnetUserExists != null)
            {
                TempData["Error"] = "This Email Id Is Already Exists!";
                return RedirectToAction("UserAccess", "Admin");

            }

            AspnetUser aspnetUser = new AspnetUser();
            aspnetUser.Username = createProvider.UserName;
            aspnetUser.Aspnetuserid = Guid.NewGuid().ToString();
            aspnetUser.Email = createProvider.Email;
            aspnetUser.Passwordhash = BC.HashPassword(createProvider.Passwordhash);
            aspnetUser.Phonenumber = createProvider.PhoneNo;
            _admin.AddAspNetUser(aspnetUser);
            _admin.SaveChanges();
            Physician physician = new Physician();
            physician.Roleid = createProvider.RoleId;
            physician.Firstname = createProvider.Firstname;
            physician.Lastname = createProvider.Lastname;
            physician.Aspnetuserid = aspnetUser.Aspnetuserid;
            physician.Email = createProvider.Email;
            physician.Mobile = createProvider.PhoneNo;
            physician.Medicallicense = createProvider.MedicalLicense;
            physician.Address1 = createProvider.Address1;
            physician.Address2 = createProvider.Address2;
            physician.Regionid = createProvider.State;
            physician.City = createProvider.City;
            physician.Zip = createProvider.ZipCode;
            physician.Createdby = id;
            physician.Createddate = DateTime.Now;
            physician.Status = 1;
            physician.Isdeleted = false;
            physician.Businessname = createProvider.BusinessName;
            physician.Businesswebsite = createProvider.BusinessWebsite;
            physician.Npinumber = createProvider.NPINumber;
            physician.Iscredentialdoc = new BitArray(new[] { false });
            physician.Syncemailaddress = createProvider.SynchronizationEmail;
            

            if (createProvider.File != null)
            {
                physician.Photo = "photo" + createProvider.File.FileName.GetExtension();
            }
            if (createProvider.IsAgreement != null)
            {
                physician.Isagreementdoc = new BitArray(new[] { true });
            }
            else
            {
                physician.Isagreementdoc = new BitArray(new[] { false });
            }
            if (createProvider.IsBackground != null)
            {
                physician.Isbackgrounddoc = new BitArray(new[] { true });
            }
            else
            {
                physician.Isbackgrounddoc = new BitArray(new[] { false });
            }
            if (createProvider.License != null)
            {
                physician.Islicensedoc = new BitArray(new[] { false });
            }
            else
            {
                physician.Islicensedoc = new BitArray(new[] { false });
            }
            if (createProvider.NonDiscoluser != null)
            {
                physician.Isnondisclosuredoc = new BitArray(new[] { true });
            }
            else
            {
                physician.Isnondisclosuredoc = new BitArray(new[] { false });
            }
            if (createProvider.IsHippa != null)
            {
                physician.Istrainingdoc = new BitArray(new[] { true });
            }
            else
            {
                physician.Istrainingdoc = new BitArray(new[] { false });
            }
            _admin.AddPhysician(physician);
            _admin.SaveChanges();
            if (createProvider.File != null)
            {
                string photo = _uploadProvider.UploadPhoto(createProvider.File, physician.Physicianid);
            }
            if (createProvider.IsAgreement != null)
            {
                string ICA = _uploadProvider.UploadDocFile(createProvider.IsAgreement, physician.Physicianid, "ICA");
            }
            if (createProvider.IsBackground != null)
            {
                string ICA = _uploadProvider.UploadDocFile(createProvider.IsBackground, physician.Physicianid, "Background");
            }
            if (createProvider.IsHippa != null)
            {
                string ICA = _uploadProvider.UploadDocFile(createProvider.IsHippa, physician.Physicianid, "Hippa");
            }
            if (createProvider.License != null)
            {
                string ICA = _uploadProvider.UploadDocFile(createProvider.License, physician.Physicianid, "License");
            }
            if (createProvider.NonDiscoluser != null)
            {
                string ICA = _uploadProvider.UploadDocFile(createProvider.NonDiscoluser, physician.Physicianid, "NonDiscoluser");
            }
            _admin.AddPhysicianNotification(physician.Physicianid);
            foreach (string item in adminRegion)
            {

                _admin.AddPhysicianRegion(physician.Physicianid, int.Parse(item));
            }
            _admin.SaveChanges();

            _admin.AddAspnetUserRole(aspnetUser.Aspnetuserid, createProvider.RoleId);
            TempData["SuccessMessage"] = "Provider Account Create Succesfully!!";
            List<PayrateCategory> df = _admin.GetPayrateCategories();
            foreach (var item in df)
            {
                var Payratebyprovider = new PayrateByProvider();
                Payratebyprovider.PayrateCategoryId = item.PayrateCategoryId;
                Payratebyprovider.PhysicianId = physician.Physicianid;
                Payratebyprovider.CreatedDate = DateTime.Now;
                Payratebyprovider.CreatedBy = admin.Aspnetuserid;
                Payratebyprovider.Payrate = 0;
                _admin.AddPayrateCategories(Payratebyprovider);
                _admin.SaveChanges();
            }



            return RedirectToAction("Provider");
        }
        #endregion

        #endregion

        #region SchedulingPage

        #region Scheduling
        [CustomAuthorize("Provider", "2", "20")]

        [HttpGet("Admin/Scheduling", Name = "AdminScheduling")]
        [HttpGet("Provider/Scheduling", Name = "ProviderScheduling")]
        public IActionResult Scheduling()
        {
            ViewBag.Regions = _admin.GetAllRegion();
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            List<Region> region = new List<Region>();

            if (admin != null)
            {
                ViewBag.IsPhysician = false;
                region = _admin.GetAllRegion();
            }
            if (physician != null)
            {
                ViewBag.IsPhysician = true;
                region = _admin.GetPhysicianWorkingRegion(physician.Physicianid);

            }
            ViewBag.regions = region;
            return View();
        }
        #endregion

        #region CreateShift

        public IActionResult CreateShift(ScheduleModel data)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            if (physician != null)
            {
                data.Physicianid = physician.Physicianid;
            }
            List<DateTime> ConflictingDates = _admin.IsShiftOverwritting(data);
            if (ConflictingDates.Count > 0)
            {
                TempData["Error"] = $"Conflicting shifts found on: {string.Join(", ", ConflictingDates.Select(d => d.ToString("dd-MM-yyyy")))}";
            }
            else
            {
                string? email = HttpContext.Session.GetString("Email");
                if (email == null)
                {
                    TempData["Error"] = "Session Is Expire Please Login!";
                    return RedirectToAction("Index", "Login");
                }
                data.Status = 1;
                //create shift here
                _admin.CreateShift(data, email);
                TempData["Success"] = "Shift created successfully";
            }
            return Redirect("Scheduling");

        }
        #endregion

        #region ProviderOnCall
        public IActionResult ProviderOnCall()
        {
            List<Region> Regions = _admin.GetAllRegion();
            ViewBag.Regions = Regions;

            return View();
        }
        #endregion

        #region ProviderOnCallFetch
        public IActionResult ProviderOnCallFetch(int region)
        {
            ProviderOnCallVM model = _admin.GetProvidersOnCall(region, 0);
            return PartialView("ProviderOnCallPartial", model);
        }
        #endregion

        #region GetPhysicianShift
        [HttpGet]
        public IActionResult GetPhysicianShift(int region)
        {
            var physicians = _admin.GetPhysiciansByRegion(region);
            return Ok(physicians);
        }
        #endregion

        #region GetEvents
        [HttpGet]
        public IActionResult GetEvents(int region)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            bool IsPhysician = false;
            List<ScheduleModel> events = new List<ScheduleModel>();
            if (admin != null)
            {
                events = _admin.GetEvents(region, IsPhysician, admin.Adminid);
                ViewBag.IsPhysician = false;
            }

            if (physician != null)
            {
                events = _admin.GetEvents(region, true, physician.Physicianid);
                ViewBag.IsPhysician = true;
            }
            var mappedEvents = events.Select(e => new
            {
                id = e.Shiftid,
                resourceId = e.Physicianid,
                title = e.Starttime.ToString() + "-" + e.Endtime.ToString() + " " + e.PhysicianName,
                start = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Starttime.Hour, e.Starttime.Minute, e.Starttime.Second),
                end = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Endtime.Hour, e.Endtime.Minute, e.Endtime.Second),
                ShiftDetailId = e.ShiftDetailId,
                region = _admin.GetRegionsByRegionId(e.Regionid),
                status = e.Status
            }).ToList();

            return Ok(mappedEvents);
        }
        #endregion

        #region EditShifts
        [HttpPost]
        public IActionResult SaveShift(int shiftDetailId, DateTime startDate, TimeOnly startTime, TimeOnly endTime, int region)
        {
            Shiftdetail? shiftdetail = _admin.GetShiftDetailById(shiftDetailId);


            if (shiftdetail == null)
            {
                return NotFound("Shift detail not found.");
            }
            bool shiftExists = _admin.ShiftExists(startDate, startTime, endTime, shiftdetail);
            if (shiftExists)
            {
                return BadRequest("Shifts Are Conflicting With Each Other Please check Befor Update");
            }
            try
            {
                shiftdetail.Shiftdate = startDate;
                shiftdetail.Starttime = startTime;
                shiftdetail.Endtime = endTime;

                _admin.UpdateShiftDetail(shiftdetail);
                _admin.SaveChanges();
                string? Email = HttpContext.Session.GetString("Email");
                List<ScheduleModel> events = new List<ScheduleModel>();

                if (Email == null)
                {
                    return BadRequest("Session Is Expire Please Reload");

                }
                Admin? admin = _admin.GetAdminByEmail(Email);
                Physician? physician = _admin.GetPhysicianByEmail(Email);
                bool IsPhysician = false;

                if (admin != null)
                {
                    events = _admin.GetEvents(region, IsPhysician, admin.Adminid);
                    ViewBag.IsPhysician = false;
                }
                if (physician != null)
                {
                    events = _admin.GetEvents(region, IsPhysician, physician.Physicianid);
                    ViewBag.IsPhysician = true;
                }
                var mappedEvents = events.Select(e => new
                {
                    id = e.Shiftid,
                    resourceId = e.Physicianid,
                    title = e.Starttime.ToString() + "-" + e.Endtime.ToString() + " " + e.PhysicianName,
                    start = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Starttime.Hour, e.Starttime.Minute, e.Starttime.Second),
                    end = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Endtime.Hour, e.Endtime.Minute, e.Endtime.Second),
                    ShiftDetailId = e.ShiftDetailId,
                    region = _admin.GetRegionsByRegionId(e.Regionid),
                    status = e.Status
                }).ToList();
                return Ok(new { message = "Shift detail updated successfully.", events = mappedEvents });
            }
            catch (Exception ex)
            {
                return BadRequest("Error updating shift detail: " + ex.Message);
            }
        }
        #endregion

        #region DeleteShift
        public IActionResult DeleteShift(int shiftDetailId, int region)
        {
            Shiftdetail? shiftdetail = _admin.GetShiftDetailById(shiftDetailId);
            if (shiftdetail == null)
            {
                return NotFound("Shift detail not found.");
            }
            shiftdetail.Isdeleted = true;
            _admin.UpdateShiftDetail(shiftdetail);
            _admin.SaveChanges();
            List<ScheduleModel> events = _admin.GetEvents(region, false, 0);
            var mappedEvents = events.Select(e => new
            {
                id = e.Shiftid,
                resourceId = e.Physicianid,
                title = e.Starttime.ToString() + "-" + e.Endtime.ToString() + " " + e.PhysicianName,
                start = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Starttime.Hour, e.Starttime.Minute, e.Starttime.Second),
                end = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Endtime.Hour, e.Endtime.Minute, e.Endtime.Second),
                ShiftDetailId = e.ShiftDetailId,
                region = _admin.GetRegionsByRegionId(e.Regionid),
                status = e.Status
            }).ToList();
            return Ok(new { message = "Shift detail Deleted successfully.", events = mappedEvents });
        }
        #endregion

        #region ReturnShift
        public IActionResult ReturnShift(int shiftDetailId, int region)
        {
            Shiftdetail? shiftdetail = _admin.GetShiftDetailById(shiftDetailId);

            // If shift detail is not found, return a 404 Not Found response
            if (shiftdetail == null)
            {
                return NotFound("Shift detail not found.");
            }
            shiftdetail.Status = (short)((shiftdetail.Status == 0) ? 1 : 0);

            _admin.UpdateShiftDetail(shiftdetail);
            _admin.SaveChanges();
            List<ScheduleModel> events = _admin.GetEvents(region, false, 0);
            var mappedEvents = events.Select(e => new
            {
                id = e.Shiftid,
                resourceId = e.Physicianid,
                title = e.Starttime.ToString() + "-" + e.Endtime.ToString() + " " + e.PhysicianName,
                start = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Starttime.Hour, e.Starttime.Minute, e.Starttime.Second),
                end = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Endtime.Hour, e.Endtime.Minute, e.Endtime.Second),
                ShiftDetailId = e.ShiftDetailId,
                region = _admin.GetRegionsByRegionId(e.Regionid),
                status = e.Status
            }).ToList();
            return Ok(new { message = "Shift detail updated successfully.", events = mappedEvents });
        }
        #endregion

        #region ShiftReview
        public IActionResult ShiftReview()
        {
            ViewBag.regions = _admin.GetAllRegion();
            return View();
        }
        #endregion

        #region GetReviewShift
        public IActionResult GetReviewShift(int region)
        {
            var shifts = _admin.GetReviewShift(region);
            return PartialView("RequestedShiftPartial", shifts);
        }
        #endregion

        #region ApprovedShifts
        [HttpPost]
        public IActionResult ApprovedShifts(List<int> selectedIds)
        {
            try
            {
                foreach (int id in selectedIds)
                {
                    Shiftdetail? shiftDetail = _admin.GetShiftDetailById(id);
                    if (shiftDetail != null)
                    {
                        shiftDetail.Status = 0; // Change the state to 0
                        _admin.UpdateShiftDetail(shiftDetail);
                        _admin.SaveChanges();
                    }
                }

                return Ok("Selected shifts have been successfully approved.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while approving selected shifts: " + ex.Message);
            }
        }
        #endregion

        #region DeleteSelectedShiftDetails
        [HttpPost]
        public IActionResult DeleteSelectedShiftDetails(List<int> selectedIds)
        {
            try
            {
                foreach (int id in selectedIds)
                {
                    Shiftdetail? shiftDetail = _admin.GetShiftDetailById(id);
                    if (shiftDetail != null)
                    {
                        shiftDetail.Isdeleted = true; // Change the state to 0
                        _admin.UpdateShiftDetail(shiftDetail);
                        _admin.SaveChanges();
                    }
                }
                return Ok("Selected shifts have been successfully Deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while Deleting selected shifts: " + ex.Message);
            }
        }
        #endregion

        #region UpdateShiftDates
        public IActionResult UpdateShiftDates()
        {
            return Ok();
        }
        #endregion

        #endregion

        #endregion

        #region PartentesPage

        #region Parteners
        public IActionResult Parteners()
        {
            List<Healthprofessionaltype> healthprofessionaltype = _admin.GetAllHealthprofessoionalType();
            ViewBag.helthprofessional = healthprofessionaltype;
            return View();
        }
        #endregion

        #region PartnerFilter
        public IActionResult PartnerFilter(int helthProType, string vendorname, int page)
        {
            var orders = _admin.PartnerFilter(helthProType, vendorname);
            int pageSize = 3;
            int totalItems = orders.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<SendOrderModel> paginatedData = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            return PartialView("PatnerPartialView", paginatedData);
        }
        #endregion

        #region DelteHealthProfession
        public IActionResult DelteHealthProfession(int id)
        {
            Healthprofessional? healthprofessional = _admin.GetHealthprofessionalById(id);
            if (healthprofessional != null)
            {
                healthprofessional.Isdeleted = true;

                _admin.UpdateHealthPrifessional(healthprofessional);
                _admin.SaveChanges();
                return Json(new { success = true, message = "The record has been deleted." });
            }
            else
            {
                return StatusCode(500, new { message = "HealthProffesion Is Not In Existes" });
            }
        }
        #endregion

        #region EditPartner
        public IActionResult EditPartner(int partnerId)
        {
            Healthprofessional? healthprofessional = _admin.GetHealthprofessionalById(partnerId);
            if (healthprofessional == null)
            {
                TempData["Error"] = "Something Is Went Wrong!!";
                return RedirectToAction("Index", "Admin");
            }

            List<Healthprofessionaltype> healthprofessionaltype = _admin.GetAllHealthprofessoionalType();
            List<Region> Regions = _admin.GetAllRegion();
            ViewBag.Regions = Regions;
            ViewBag.helthprofessional = healthprofessionaltype;
            HealthProffesionalVM healthProffesionalVM = new HealthProffesionalVM();
            healthProffesionalVM.Businesscontact = healthprofessional?.Businesscontact ?? "";
            healthProffesionalVM.Profession = (int)healthprofessional.Healthprofessionalid;
            healthProffesionalVM.Vendorname = healthprofessional.Vendorname;
            healthProffesionalVM.Faxnumber = healthprofessional.Faxnumber;
            healthProffesionalVM.Address = healthprofessional?.Address ?? "";
            healthProffesionalVM.City = healthprofessional?.City ?? "";
            healthProffesionalVM.State = healthprofessional?.State ?? "";
            healthProffesionalVM.Zip = healthprofessional?.Zip ?? "";
            healthProffesionalVM.Email = healthprofessional?.Email ?? "";
            healthProffesionalVM.Phonenumber = healthprofessional?.Phonenumber ?? "";
            healthProffesionalVM.VendorId = healthprofessional.Vendorid;
            return View(healthProffesionalVM);
        }
        #endregion


        #region EditPartnerPost
        [HttpPost]
        public IActionResult EditPartner(HealthProffesionalVM healthProffesionalVM)
        {
            Healthprofessional? healthprofessional = _admin.GetHealthprofessionalById(healthProffesionalVM.VendorId);
            AspnetUser? aspnetUser = _patient.GetAspnetUserBYEmail(healthProffesionalVM.Email);

            if (healthprofessional == null)
            {
                TempData["Error"] = "Something Is Went Wrong!!";

                return RedirectToAction("Index", "Admin");
            }

            if (healthprofessional.Email != healthProffesionalVM.Email)
            {

                if (aspnetUser != null)
                {
                    TempData["Error"] = "Please Check Email Its Already Exist!!";
                    return RedirectToAction("Index", "Admin");

                }
            }
            Region? region = _admin.GetRegionByName(healthProffesionalVM.State);
            if (region != null)
            {
                healthprofessional.Regionid = region.Regionid;
            }


            healthprofessional.Vendorname = healthProffesionalVM.Vendorname;
            healthprofessional.Healthprofessionalid = healthProffesionalVM.Profession;
            healthprofessional.Email = healthProffesionalVM.Email;
            healthprofessional.Businesscontact = healthProffesionalVM.Businesscontact;
            healthprofessional.Address = healthProffesionalVM.Address;
            healthprofessional.Phonenumber = healthProffesionalVM.Phonenumber;
            healthprofessional.City = healthProffesionalVM.City;
            healthprofessional.State = healthProffesionalVM.State;
            healthprofessional.Zip = healthProffesionalVM.Zip;
            healthprofessional.Faxnumber = healthProffesionalVM.Faxnumber;
            _admin.UpdateHealthPrifessional(healthprofessional);
            _admin.SaveChanges();
            TempData["SuccessMessage"] = "Details Update SuccessFully!!";
            return RedirectToAction("EditPartner", new { partnerId = healthprofessional.Vendorid });
        }
        #endregion


        #region CreatePatner
        public IActionResult CreatePatner()
        {
            List<Healthprofessionaltype> healthprofessionaltype = _admin.GetAllHealthprofessoionalType();
            List<Region> Regions = _admin.GetAllRegion();
            ViewBag.Regions = Regions;
            ViewBag.helthprofessional = healthprofessionaltype;
            return View();
        }
        #endregion

        #region CreatePatnerPost
        [HttpPost]
        public IActionResult CreatePatner(HealthProffesionalVM healthProffesionalVM)
        {
            _admin.CreatePartner(healthProffesionalVM);
            return RedirectToAction("Parteners");
        }
        #endregion

        #endregion

        #region RecordsDeorpDown

        #region Records
        public IActionResult Records()
        {
            return View();
        }
        #endregion


        #region GetPatientHistoryData
        public IActionResult GetPatientHistoryData(string firstName, string lastName, string email, string phoneNumber, int page)
        {
            List<User> users = _admin.GetUsers(firstName, lastName, email, phoneNumber);
            if (users == null)
            {
                TempData["Error"] = "Something Is Went Wrong!!";
                return RedirectToAction("Index", "Admin");
            }
            List<PatientHistoryVM> patientHistoryVms = users.Select(user => new PatientHistoryVM
            {
                FirstName = user.Firstname,
                LastName = user?.Lastname ?? "",
                Email = user?.Email ?? "",
                PhoneNumber = user?.Mobile ?? "",
                Address = user?.City ?? "",
                UserId = user?.Userid ?? 0
            }).ToList();
            int pageSize = 3;
            int totalItems = patientHistoryVms.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<PatientHistoryVM> paginatedData = patientHistoryVms.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            return PartialView("PatientHistoryPartial", paginatedData);
        }
        #endregion

        #region PatientRecords
        public IActionResult PatientRecords(int userId)
        {
            List<PatientHistoryVM> patientRecords = _admin.GetPatientRecords(userId);
            return View(patientRecords);
        }
        #endregion



        #region GetPatientSearchRecords
        public IActionResult GetPatientSearchRecords(int[] status, string patientName, int requestType, string providorName, string email, string phoneNumber, bool exportStatus, int page, DateTime fromDos, DateTime toDos)
        {
            var searchRecord = _admin.GetSearchRecords(email, phoneNumber, patientName, providorName, status, requestType, fromDos, toDos);
            int pageSize = 10;
            int totalItems = searchRecord.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<SearchRecordVM> paginatedData = searchRecord.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            if (exportStatus)
            {
                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(searchRecord);
                    writer.Flush();
                    var result = memoryStream.ToArray();
                    return File(result, "text/csv", "filtered_data.csv"); // Change content type and file name as needed
                }
            }

            return PartialView("SearchRecordsPartial", paginatedData);
        }
        #endregion


        public IActionResult DeletePatient(int id)
        {
            Request? request = _admin.GetRequestById(id);
            if (request != null)
            {
                request.Isdeleted = true;
                _admin.UpdateRequest(request);
            }
            _admin.SaveChanges();
            return RedirectToAction("SearchRecords");
        }




        public IActionResult BlockHistory()
        {
            return View();
        }


        public IActionResult GetBLockHistory(string name, string email, string phonenumber, int page)
        {
            List<Blockrequest>? blockdata = _admin.GetBlockRequests(name, email, phonenumber);
            int pageSize = 3;
            int totalItems = blockdata.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<Blockrequest> paginatedData = blockdata.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return PartialView("BlockHistoryPartial", paginatedData);
        }

        public IActionResult UnBlockUser(int id)
        {
            string email = HttpContext.Session.GetString("Email");
            Admin? admin = _admin.GetAdminByEmail(email);
            Blockrequest? blockHistory = _admin.GetBlockrequestById(id);
            if (blockHistory != null)
            {
                blockHistory.Isactive = false;
                blockHistory.Request.Status = 1;
                blockHistory.Modifieddate = DateTime.Now;
                blockHistory.Request.Physicianid = null;

                _admin.UpdateBlockRequest(blockHistory);
                _admin.SaveChanges();
                TempData["SuccessMessage"] = "UnBlock SuccessFull!!";

            }
            else
            {
                TempData["Error"] = "There Are No BlockRequestExist!!";

            }
            return RedirectToAction("BlockHistory");
        }
        #endregion

        public IActionResult EmailLogs()
        {
            return View();
        }

        public IActionResult GetEmailLogs(int accounttype, string receivername, string emailid, DateTime createddate, DateTime sentdate)
        {
            List<LogsVM> logs = _admin.GetEmailLogs(accounttype, receivername, emailid, createddate, sentdate);
            return PartialView("EmailLogsPartial", logs);
        }

        public IActionResult SmsLogs()
        {
            return View();
        }

        public IActionResult GetSMSLogs(int currentPage, int pageSize, int role, string reciever, string mobile, DateTime createdDate, DateTime sentDate)
        {
            List<LogsVM> logs = _admin.GetSmsLogs(role, reciever, mobile, createdDate, sentDate);
            return PartialView("SmsLogsPartial", logs);
        }
        public IActionResult SearchRecords()
        {
            return View();
        }

        #region AccessDropDown

        #region AccessPage


        #region Access
        /// <summary>
        /// Its Showed The All Role Are Present In The Database
        /// </summary>

        [CustomAuthorize("Role", "7")]
        public IActionResult Access()
        {
            //This Page Show The All Role In DataBase
            List<Role> roles = _admin.GetAllRoles();
            var list = roles.Where(item => item.Isdeleted != null && (item.Isdeleted.Length == 0 || !item.Isdeleted[0]));
            return View(list.ToList());
        }
        #endregion

        #region CreateAccess
        [CustomAuthorize("Role", "7")]
        public IActionResult CreateAccess()
        {
            //Create Access View Page
            return View();
        }
        #endregion

        #region CreateAccessPost
        [CustomAuthorize("Role", "7")]
        [HttpPost]
        public IActionResult CreateAccess(int[] rolemenu, string rolename, int accounttype)
        {
            //New Role Is Add In Role Table And Role-Menu Table
            Role role = new Role();
            role.Name = rolename;
            role.Accounttype = (short)accounttype;
            role.Createdby = "admin";
            role.Createddate = DateTime.Now;
            role.Isdeleted = new BitArray(new[] { false });
            _admin.AddRoles(role);
            _admin.SaveChanges();

            foreach (int menu in rolemenu)
            {
                Rolemenu rolemenu1 = new Rolemenu();
                rolemenu1.Menuid = menu;
                rolemenu1.Roleid = role.Roleid;
                _admin.AddRoleMenus(rolemenu1);
                _admin.SaveChanges();

            }
            return RedirectToAction("Access");
        }
        #endregion

        #region RoleData
        [CustomAuthorize("Role", "7")]
        public IActionResult RoleData(int region)
        {
            //Role Data Is Ajax Method That Give All Role In Present In Database
            List<Menu> menuList = _admin.GetMenuByAccountType(region);
            return PartialView("accesspartial", menuList);
        }
        #endregion

        #region EditRoleData
        [CustomAuthorize("Role", "7")]
        public IActionResult EditRoleData(int region, int roleid)
        {
            //Onclick The Edit Btn It Will Show EditRole View 
            //Menulist Give The List Of All Menu Present In The Menu Table
            //Region="Patient","Admin","Physician"
            List<Menu> menuList = _admin.GetMenuByAccountType(region);

            //rolemenu give The Menuid of the particular role Has How Many Menuid
            List<int>? rolemenu = _admin.GetRoleMenuIdByRoleId(roleid);

            RoleMenuViewModel viewModel = new RoleMenuViewModel
            {
                MenuList = menuList,
                RoleMenuIds = rolemenu
            };

            return PartialView("EditAccessPartial", viewModel);
        }
        #endregion

        #region EditAccess
        [CustomAuthorize("Role", "7")]
        public IActionResult EditAccess(int roleid)
        {
            try
            {
                // Retrieve menu IDs associated with the given role ID
                List<int> rolemenu = _admin.GetRoleMenuIdByRoleId(roleid);

                // Check if the retrieved list is null
                if (rolemenu == null)
                {
                    TempData["Error"] = "Error retrieving menu IDs for the role.";
                    return RedirectToAction("Access", "Admin");
                }

                // Retrieve role details
                Role role = _admin.GetAllRolesById(roleid);

                // Check if the retrieved role is null
                if (role == null)
                {
                    TempData["Error"] = "Role not found.";
                    return RedirectToAction("Access", "Admin");
                }

                // Construct AccessVM model
                AccessVM accessVM = new AccessVM
                {
                    Menu = rolemenu,
                    Name = role.Name,
                    Roleid = roleid,
                    Accounttype = role.Accounttype
                };

                // Return view with AccessVM model
                return View(accessVM);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An unexpected error occurred: " + ex.Message;
                return RedirectToAction("Access", "Admin");
            }
        }

        #endregion

        #region EditAccessPost
        [CustomAuthorize("Role", "7")]
        [HttpPost]
        public IActionResult EditAccess(int roleid, int[] rolemenu, string rolename, int accounttype)
        {

            Role role = _admin.GetAllRolesById(roleid);

            //rolemenu give The Menuid of the particular role Has How Many Menuid
            List<Rolemenu> menulist = _admin.GetRoleMenuById(roleid);
            role.Name = rolename;
            role.Accounttype = (short)accounttype;
            _admin.UpdateRoles(role);
            _admin.SaveChanges();
            _admin.RemoveRangeRoleMenu(menulist);
            _admin.SaveChanges();
            foreach (int item in rolemenu)
            {
                Rolemenu rolemenu1 = new Rolemenu();
                rolemenu1.Menuid = item;
                rolemenu1.Roleid = roleid;
                _admin.AddRoleMenus(rolemenu1);
            }
            _admin.SaveChanges();
            TempData["SuccessMessage"] = "Your Role Update SuccessFull!!";
            return RedirectToAction("Access", new { roleid = roleid });
        }
        #endregion

        #region DeleteRole
        [CustomAuthorize("Role", "7")]
        public IActionResult DeleteRole(int roleId)
        {
            //Delete Role Is Used To Not Disply The Role In View 
            Role? role = _admin.GetAllRolesById(roleId);
            if (role != null)
            {
                role.Isdeleted = new BitArray(new[] { true });
                _admin.UpdateRoles(role);
                _admin.SaveChanges();
                TempData["SuccessMessage"] = "Your Role Has Been Deleted";
            }
            else
            {
                TempData["Error"] = "Your Role Has Been Not Deleted";
            }
            return RedirectToAction("access");
        }
        #endregion

        #endregion

        #region UserAccessPage

        #region UserAccessView
        public IActionResult UserAccess()
        {
            return View();
        }
        #endregion

        #region UserData
        public IActionResult UserData(int role)
        {
            List<UserAccess> list = _admin.GetUserData(role);
            return PartialView("UserAccessPartial", list);
        }
        #endregion

        #region CreateAdmin
        public IActionResult CreateAdmin()
        {
            List<Role> roles = _admin.GetRoleFromAccountType(1);
            List<Region> region = _admin.GetAllRegion();
            AdminProfileVm profile = new AdminProfileVm();
            profile.Regions = region;
            profile.Roles = roles;
            return View(profile);
        }
        #endregion

        #region CreateAdminPost
        [HttpPost]
        public IActionResult CreateAdmin(AdminProfileVm profileVm, int[] adminRegion)
        {
            AspnetUser? aspnetUser = _admin.GetAspNetUserByEmail(profileVm.Email);
            if (aspnetUser != null)
            {
                TempData["Error"] = "Send Request Unsuccessful!";
                return RedirectToAction("CreateAdmin", "Admin");
            }


            AspnetUser aspnet = new AspnetUser();
            aspnet.Email = profileVm?.Email ?? "";
            aspnet.Aspnetuserid = Guid.NewGuid().ToString();
            aspnet.Username = profileVm?.Username ?? "";
            aspnet.Createddat = DateTime.Now;
            aspnet.Passwordhash = BC.HashPassword(profileVm?.Password ?? "");
            aspnet.Phonenumber = profileVm?.MobileNo ?? "";
            _admin.AddAspNetUser(aspnet);
            _admin.SaveChanges();

            Admin admin = new Admin();
            admin.Email = profileVm?.Email ?? "";
            admin.Aspnetuserid = aspnet.Aspnetuserid;
            admin.Firstname = profileVm?.FirstName ?? "";
            admin.Lastname = profileVm?.LastName ?? "";
            admin.Mobile = profileVm?.MobileNo ?? "";
            admin.Address2 = profileVm?.Address2 ?? "";
            admin.Address1 = profileVm?.Address1 ?? "";
            admin.City = profileVm?.City ?? "";
            admin.Regionid = profileVm.State;
            admin.Zip = profileVm.ZipCode;
            admin.Createdby = aspnet.Aspnetuserid;
            admin.Status = 1;
            admin.Isdeleted = false;

            _admin.AddAdmin(admin);
            _admin.SaveChanges();

            foreach (int region in adminRegion)
            {
                _admin.AddAdminRegion(admin.Adminid, region);
            }
            _admin.AddAspnetUserRole(aspnet.Aspnetuserid, profileVm.RoleAdminId);

            return RedirectToAction("UserAccess", "Admin");
        }
        #endregion

        #endregion

        #endregion


        #region Admin&ProviderDashboard

        #region IndexWithOutDashboardPageAccess
        [HttpGet("Admin/Index", Name = "AdminIndex")]
        public IActionResult IndexWithOutDashboard()
        {
            //If Admin have not access Of Dashboard then it will redirct here  
            ViewData["Title"] = "Dashboard";
            string? Email = HttpContext.Session.GetString("Email");
            //check the session data if someone try to access it without login then it will show message
            if (Email == null)
            {
                TempData["Error"] = "Your Session Will Be Expire Please LogedIn Again";
                return RedirectToAction("Index", "Login");
            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            //if someone login and then its not admin then it will show error message
            if (admin == null)
            {
                TempData["Error"] = "Your Session Will Be Expire Please LogedIn Again";
                return RedirectToAction("Index", "Login");

            }
            string? rolefromroleid = HttpContext.Session.GetString("Role");
            if (rolefromroleid == null)
            {
                TempData["Error"] = "Your Session Will Be Expire Please LogedIn Again";
                return RedirectToAction("Index", "Login");

            }
            List<string> menulist = _admin.GetMenuNamesByRoleId(int.Parse(rolefromroleid));
            //Check The RoleList If some admin with dashboard access try to access this page then it will redirect to dashboard
            if (menulist.Contains("Dashboard"))
            {
                List<NewRequestTableVM> request = _admin.GetAllData();
                int newcount = request.Count(item => item.Status == 1);
                return RedirectToAction("Index");
            }
            //else show Its View
            return View();
        }
        #endregion

        #region Index
        [HttpGet("Admin/Dashboard", Name = "AdminDashboard")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            List<Region> regions = _admin.GetAllRegion();

            ViewBag.regions = regions;

            string? rolefromroleid = HttpContext.Session.GetString("Role");
            if (rolefromroleid == null)
            {
                TempData["Error"] = "Your Session Will Be Expire Please LogedIn Again";
                return RedirectToAction("Index", "Login");

            }
            List<string> menulist = _admin.GetMenuNamesByRoleId(int.Parse(rolefromroleid));
            //Check The RoleList If its contain dashboard in menu then it will show dashboard
            if (menulist.Contains("Dashboard"))
            {
                List<NewRequestTableVM> request = _admin.GetAllData();
                int newcount = request.Count(item => item.Status == 1);
                return View(request.ToList());
            }
            //else it will redirect to indexwithoutpage
            else
                return RedirectToAction("IndexWithOutDashboard");
        }
        #endregion

        #region SearchPatientDashboard
        [CustomAuthorize("Dashboard", "6")]
        public IActionResult SearchPatient(string searchValue, string selectValue, string partialName, string selectedFilter, int[] currentStatus, bool exportdata, bool exportAllData, int page, int pageSize = 3)
        {
            //Its Define ALl Cancel Reason Theat Stored In Database
            ViewBag.Cancel = _admin.GetCancelCases();
            //For Pagination Error If Page Not Found Then It show Firstpage
            if (page == 0)
            {
                page = 1;
            }
            //Search Value ==Filter The Database Accoeding To Its name If Admin Search name Theat Start With 'm'
            //Select Value == filter The DataAccoeding TO Its Region
            //Select Filter == filter The DataAccoeding To Its RequestTypeId ,Ex:'Patient','Friend','Conciege','Family Friend01'
            //Pass The Current Status If Admin Click In New Then It Will Passed Status=[1],panding=[2],active=[4,5],conclude=[8],toclose=[3,7,9],unpaid=[10]

            List<NewRequestTableVM> filteredPatients = _admin.SearchPatients(searchValue, selectValue, selectedFilter, currentStatus);

            //Export The Data
            List<NewRequestTableVM> ExportData = _admin.SearchPatients(searchValue, selectValue, selectedFilter, currentStatus);

            //Total Item In Filterpatients Result 
            int totalItems = filteredPatients.Count();
            //Count TotalPage
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<NewRequestTableVM> paginatedData = filteredPatients.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalEntries = totalItems;

            //If admin Click Exportdata  Then it download the excel File 
            if (exportdata)
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
            ViewBag.IsPhysician = false;
            return PartialView(partialName, paginatedData);
        }
        #endregion

        #region GetAllRegion
        public ActionResult<IEnumerable<Region>> GetRegions()
        {
            return _admin.GetAllRegion();
        }
        #endregion

        #region ExportAll
        [CustomAuthorize("Dashboard", "6")]
        public IActionResult ExportAll(string currentStatus)
        {
            //ExportAll Data Are ExportAll Data In That Paticular Data If State Is New Then It will Make Excel Of That state All Data
            int[]? statusArray = currentStatus?.Split(',')?.Select(int.Parse).ToArray();
            string searchValue = "";
            string selectValue = "";
            string selectedFilter = "";
            List<NewRequestTableVM> filteredPatientsd = _admin.SearchPatients(searchValue, selectValue, selectedFilter, statusArray);
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(filteredPatientsd);
                writer.Flush();
                var result = memoryStream.ToArray();
                return File(result, "text/csv", "filtered_data.csv");
            }
        }
        #endregion

        #region ViewCasePage

        #region ViewCaseGet

        [CustomAuthorize("Dashboard", "6", "19")]
        [HttpGet("Admin/viewcase/{id}", Name = "AdminViewCase")]
        [HttpGet("Provider/viewcase/{id}", Name = "ProviderCase")]
        public IActionResult ViewCase(int id)
        {
            //In This Id Is RequestClient Id 
            // Physician And Admin Both Are Used The Same ViewCase Page
            string? Email = HttpContext.Session.GetString("Email");
            ViewBag.Cancel = _admin.GetCancelCases();
            ViewBag.Regions = _admin.GetAllRegion();
            if (Email == null)
            {
                TempData["Error"] = "Your Session Will Be Expire Please LogedIn Again";
                return RedirectToAction("Index", "Login");
            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            //Get Data From The RequestClient Table
            Requestclient? request = _admin.GetRequestClientById(id);
            ViewCaseVM ViewCase = new ViewCaseVM();
            //If requestclient Details Is Null Or Not IF Null Then Redirect To The Dashboard Page
            if (request == null)
            {
                TempData["Error"] = "Something Went Wrong";
                if (admin != null)
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Provider");
            }
            if (admin != null)
            {
                ViewBag.IsPhysican = false;

                ViewCase = _admin.GetCaseById(id, 0);
            }
            else if (physician != null)
            {
                ViewBag.IsPhysican = true;
                if (request.Request.Physicianid != physician.Physicianid)
                {
                    //Physician Is Try To Access Other Patient ViewCase Detials 
                    TempData["Error"] = "Dont Try To Access It!!";
                    return RedirectToAction("Index", "Provider");
                }
                ViewCase = _admin.GetCaseById(id, physician.Physicianid);

            }
            return View(ViewCase);

        }
        #endregion

        #region ViewCasePost
        [CustomAuthorize("Dashboard", "6")]
        [HttpPost]
        public async Task<IActionResult> ViewCase(ViewCaseVM viewCaseVM, int id)
        {

            if (ModelState.IsValid)
            {
                //Update Viewcase Detials
                Requestclient? requestclient = _admin.GetRequestClientById(id);
                //If Enter Email Is Already Exists In database Or Not Is Checked
                if (requestclient.Email != viewCaseVM.EmailView)
                {
                    AspnetUser? aspnetUser = _admin.GetAspNetUserByEmail(viewCaseVM.EmailView);
                    if (aspnetUser != null)
                    {
                        TempData["Error"] = "This Email Is Already Exsits In DataBase";
                        return RedirectToAction("ViewCase", new { id });
                    }
                }
                await _admin.UpdateRequestClient(viewCaseVM, id);
                return RedirectToAction("ViewCase", new { id });
            }
            return View();
        }
        #endregion

        #endregion

        #region ViewNotesPage

        #region ViewNotes
        [CustomAuthorize("Dashboard", "6", "19")]
        [HttpGet("Admin/Viewnotes", Name = "AdminNotes")]
        [HttpGet("Provider/Viewnotes", Name = "ProviderNotes")]
        public IActionResult Viewnotes(int requestid)
        {
            // Physician And Admin Both Are Used The Same ViewCase Page
            //Check The Session Details If Session Is Exist Or Not
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            //Get Session Logged User Data
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            Requestclient? request = _admin.GetRequestclientByRequestId(requestid);
            //If Request Is not IN database Then Show Error And Redirct To The Login
            if (request == null)
            {
                TempData["Error"] = "Something Went Wrong";
                if (admin != null)
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Provider");
            }
            //If Admin Is Logged In then It Pass Viewbag It include Some More Funcation Like Edit btn For Admin
            if (admin != null)
                ViewBag.IsPhysician = false;

            else if (physician != null)
            {
                ViewBag.IsPhysician = true;
                if (request.Request.Physicianid != physician.Physicianid)
                {
                    TempData["Error"] = "Dont Not Acess Other Request";
                    return RedirectToAction("Index", "Provider");
                }

            }
            ViewData["ViewName"] = "Dashboard";
            ViewData["RequestId"] = requestid;
            //Get Nots From The Repository
            ViewNotes result = _admin.GetNotesForRequest(requestid);

            return View(result);
        }
        #endregion

        #region ViewNotesPost
        [HttpPost]
        [CustomAuthorize("Dashboard", "6", "19")]
        public async Task<IActionResult> ViewNotesPost(string adminNotes, int id)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            bool IsPhysician = false;
            if (adminNotes != null)
            {
                if (admin != null)
                {
                    //Update The Notes In RequestNotes Table Admin Notes;
                    //Here IsPhysician Is False
                    await _admin.UpdateAdminNotes(id, adminNotes, admin.Aspnetuserid, IsPhysician);
                    return RedirectToAction("ViewNotes", new { requestid = id });
                }
                if (physician != null && physician.Aspnetuserid != null)
                {

                    //Update The Notes In RequestNotes Table Physician Notes;
                    //Here IsPhysician Is True
                    await _admin.UpdateAdminNotes(id, adminNotes, physician.Aspnetuserid, IsPhysician = true);
                    return RedirectToAction("ViewNotes", "Provider", new { requestid = id });
                }
            }
            else
            {
                //Handle The Error If Notes Is Not Found
                TempData["Error"] = "Notes Is Not Found Try Again!!";
                return RedirectToAction("ViewNotes", new { requestid = id });

            }
            return RedirectToAction("ViewNotes", new { requestid = id });
        }
        #endregion

        #endregion

        #region ActionsModal

        #region AssignRequest
        [CustomAuthorize("Dashboard", "6")]
        [HttpPost]
        public async Task<IActionResult> AssignRequest(int regionid, int physician, string description, int requestid, int status)
        {
            //Assign RequestModal In That Admin Assign The Patient To Then Physician 
            //Check The Session
            string? email = HttpContext.Session.GetString("Email");
            int? id = HttpContext.Session.GetInt32("id");
            if (email == null)
            {
                TempData["Error"] = "Session Is Expire Please Login!";
                return RedirectToAction("Index", "Login");
            }
            Admin? admin = _admin.GetAdminByEmail(email);
            if (admin != null)
            {
                //In This It Assgin The Physician To Patient In THis State Is Not
                //Changed After That Physician Show That Request In Physician Dashboard In New State
                bool result = await _admin.AssignRequest(regionid, physician, description, requestid, admin.Adminid);
                TempData["SuccessMessage"] = "Assign successfully";

                return Json(result);
            }
            else
            {
                return Json(false);
            }
        }
        #endregion

        #region TransferRequest
        [HttpPost]
        [CustomAuthorize("Dashboard", "6")]
        public async Task<IActionResult> TransferRequest(int regionid, int physician, string description, int requestid, int status)
        {
            string? email = HttpContext.Session.GetString("Email");
            int? id = HttpContext.Session.GetInt32("id");
            if (email == null)
            {
                TempData["Error"] = "Session Is Expire Please Login!";
                return RedirectToAction("Index", "Login");
            }
            Admin? admin = _admin.GetAdminByEmail(email);
            if (admin != null)
            {
                //Transfer Request Is In That Admin Transfer Request To The Another Physician It WIll Redirect To The New State After Transfer
                bool result = await _admin.AssignRequest(regionid, physician, description, requestid, admin.Adminid);
                return Json(result);
            }
            else
            {
                return Json(false);
            }
        }
        #endregion

        #region CloseCaseModal
        public IActionResult CloseCaseModal(int requestid)
        {
            //Close Case Modal Is Visible In When Admin Is Click In Close Case In ToClose State 
            //After Close The case Request Is Go On Unpaid State
            Request? request = _admin.GetRequestById(requestid);
            string? email = HttpContext.Session.GetString("Email");
            int? id = HttpContext.Session.GetInt32("id");
            if (email == null)
            {
                TempData["Error"] = "Session Is Expire Please Login!";
                return RedirectToAction("Index", "Login");
            }

            Admin? admin = _admin.GetAdminByEmail(email);
            if (request != null)
            {
                request.Status = 9;
            }
            Requeststatuslog requeststatuslog = new Requeststatuslog();
            requeststatuslog.Status = 9;
            requeststatuslog.Createddate = DateTime.Now;
            requeststatuslog.Requestid = requestid;
            requeststatuslog.Adminid = admin?.Adminid;
            _admin.AddRequestStatusLog(requeststatuslog);
            _admin.SaveChanges();
            TempData["SuccessMessage"] = "Close Case successfully";
            return RedirectToAction("Index", "Admin");
        }
        #endregion

        #region CancelCasePost
        [HttpPost]
        [CustomAuthorize("Dashboard", "6")]
        public async Task<IActionResult> CancelCase(string notes, string cancelReason, int requestid)
        {
            //Cancel Case Is Admin Can Cancel In New,panding State 
            //After Cancel Request Is Go ON cancel Case It Will Showd In To Close State
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            //It Change update The Notes And Cancel The Request
            bool result = await _admin.CancelCase(requestid, notes, cancelReason, admin.Adminid);
            TempData["SuccessMessage"] = "Cancel successfully";
            return Json(result);
        }
        #endregion

        #region BlockRequest
        [HttpPost]
        public IActionResult BlockRequest(string blockreason, int requestid)
        {
            //It Will Visible In admin New State WHere Admin Can BLock The Request After That User Will Not Able To Create Request With That Partclure Email;

            bool success = _admin.BlockRequest(blockreason, requestid);
            if (success)
            {
                TempData["SuccessMessage"] = "Block request successful!";
                return Ok();
            }
            else
            {
                TempData["Error"] = "Block request Unsuccessful!";
                return Ok("BLock Unsuccessful");
            }
        }
        #endregion

        #region SendLinkOfRequest

        public IActionResult SendLink(string Email, string PhoneNo, string LastNameSendOrder, string FirstNameSendOrder)
        {
            //This Page Will Send The Email Of Thew Create Request Page To The Particlur Email That Admin Fill In the FOrm 
            string? SessionEmail = HttpContext.Session.GetString("Email");
            if (SessionEmail == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(SessionEmail);
            Physician? physician = _admin.GetPhysicianByEmail(SessionEmail);

            string? resetLink = Url.ActionLink("PatientRequest", "Request", protocol: HttpContext.Request.Scheme);
            string to = Email;
            string body = $"Click <a href='{resetLink}'>here</a> to Create A new Request";
            string subject = "RequestLink";
            if (_login.IsSendEmail(Email, subject, body))
            {
                if (admin != null)
                {
                    TempData["SuccessMessage"] = "Send Request successful!";
                    _emailService.EmailLog(to, body, subject, FirstNameSendOrder + " " + LastNameSendOrder, admin.Roleid, 0, admin.Adminid, 0, 0, true, 1);
                    return RedirectToAction("Index", "Admin");
                }
                else if (physician != null)
                {
                    TempData["SuccessMessage"] = "Send Request successful!";
                    _emailService.EmailLog(to, body, subject, FirstNameSendOrder + " " + LastNameSendOrder, physician.Roleid, 0, 0, physician.Physicianid, 0, true, 1);
                    return RedirectToAction("Index", "Provider");

                }
                else
                {
                    TempData["Error"] = "Send Request Unsuccessful!";
                    return RedirectToAction("Index", "Admin");

                }

            }
            else
            {
                TempData["Error"] = "Send Request Unsuccessful!";
                if (admin != null)
                    return RedirectToAction("Index", "Admin");
                else if (physician != null)
                    return RedirectToAction("Index", "Provider");
                else
                    return RedirectToAction("Index", "Login");

            }
        }
        #endregion

        #endregion

        #region CreateRequest
        [CustomAuthorize("Dashboard", "6", "19")]
        [HttpGet("Admin/createrequest", Name = "AdminCreateRequest")]
        [HttpGet("Provider/createrequest", Name = "ProviderCreateRequest")]
        public IActionResult CreateRequest()
        {
            string? email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(email);
            Physician? physician = _admin.GetPhysicianByEmail(email);
            if (admin != null)
            {
                ViewBag.IsPhysician = false;
            }
            if (physician != null)
            {
                ViewBag.IsPhysician = true;
            }
            RequestModel requestmoel = new RequestModel();
            List<Region> region = _admin.GetAllRegion();
            requestmoel.Regions = region;
            return View(requestmoel);
        }
        #endregion



        #region CreateRequestPost
        [CustomAuthorize("Dashboard", "6", "19")]
        public IActionResult CreateRequest(RequestModel requestModel)
        {

            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            List<Region> regions = _admin.GetAllRegion();
            requestModel.Regions = regions;
            //Check Thr Block Request If User Is Alreday Blocked Then It Can Not Be Added Request Via Admin 


            if (ModelState.IsValid)
            {
                int? id = HttpContext.Session.GetInt32("id");
                Region? statebyregionid = _admin.GetRegionByName(requestModel.State);

                //Add Request In Request Table
                _patient.AddRequest(requestModel, 0, 1);
                Request? request1 = _patient.GetRequestByEmail(requestModel.Email);
                if (request1 != null)
                {
                    //Add Requestclient Table 
                    _patient.AddRequestClient(requestModel, request1.Requestid);


                    _admin.SaveChanges();

                    Requestnote requestnote = new Requestnote();


                    if (admin != null)
                    {
                        requestnote.Adminnotes = requestModel.Notes;
                        requestnote.Createdby = admin.Aspnetuserid;
                    }
                    if (physician != null && physician.Aspnetuserid != null)
                    {
                        Requeststatuslog requeststatuslog = new Requeststatuslog();
                        requeststatuslog.Status = 2;
                        requeststatuslog.Requestid = request1.Requestid;
                        requeststatuslog.Createddate = DateTime.Now;
                        requeststatuslog.Physicianid = physician.Physicianid;
                        requeststatuslog.Notes = "Physician Created Account";
                        _admin.AddRequestStatusLog(requeststatuslog);
                        _admin.SaveChanges();

                        requestnote.Physiciannotes = requestModel.Notes;
                        requestnote.Createdby = physician.Aspnetuserid;
                        request1.Physicianid = physician.Physicianid;
                        request1.Status = 2;
                        request1.Createddate = DateTime.Now;
                    }

                    requestnote.Createddate = DateTime.Now;
                    requestnote.Requestid = request1.Requestid;

                    _admin.AddRequestNotes(requestnote);
                    _admin.SaveChanges();

                    //Genrate Confirmation Number
                    int count = _patient.GetRequestCountByDate(request1.Createddate);
                    Region region = _admin.GetRegionByName(requestModel.State);
                    if (region != null)
                    {
                        string confirmNum = string.Concat(region?.Abbreviation?.ToUpper(), request1.Createddate.ToString("ddMMyy"), requestModel.Lastname.Substring(0, 2).ToUpper() ?? "",
                       requestModel.Firstname.Substring(0, 2).ToUpper(), count.ToString("D4"));
                        request1.Confirmationnumber = confirmNum;
                    }
                    else
                    {
                        string confirmNum = string.Concat("ML", request1.Createddate.ToString("ddMMyy"), requestModel.Lastname.Substring(0, 2).ToUpper() ?? "",
                      requestModel.Firstname.Substring(0, 2).ToUpper(), count.ToString("D4"));
                        request1.Confirmationnumber = confirmNum;
                    }

                    _admin.UpdateRequest(request1);
                    _admin.SaveChanges();
                    string token = Guid.NewGuid().ToString();
                    string? resetLink = Url.Action("Index", "Register", new { RequestId = request1.Requestid, token }, protocol: HttpContext.Request.Scheme);
                    string to = requestModel.Email;
                    string body = $"Click <a href='{resetLink}'>here</a> to Create A new Account";
                    string subject = "Create Your New Account";
                    if (_login.IsSendEmail(to, subject, body))
                    {

                        TempData["SuccessMessage"] = "Create Request successful!";
                        if (admin != null)
                        {
                            _emailService.EmailLog(to, body, subject, requestModel.Firstname + " " + requestModel.Lastname, admin.Roleid, 0, admin.Adminid, 0, 0, true, 1);
                        }
                        else if (physician != null)
                        {
                            _emailService.EmailLog(to, body, subject, requestModel.Firstname + " " + requestModel.Lastname, physician.Roleid, 0, 0, physician.Physicianid, 0, true, 1);
                        }
                    }
                    else
                    {
                        TempData["Error"] = " Request Unsuccessful!";

                    }
                    if (admin != null)
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "Provider");
                }


            }
            if (admin != null)
            {
                ViewBag.IsPhysician = false;
            }
            else
            {
                ViewBag.IsPhysician = true;

            }
            RequestModel requestmoel = new RequestModel();
            List<Region> regionall = _admin.GetAllRegion();
            requestmoel.Regions = regionall;
            return View(requestModel);
        }
        #endregion

        #region GetStatusCount

        public IActionResult GetStatusCounts(int id)
        {
            var countsdetails = _admin.GetStatusCountsAsync(id);

            var counts = new
            {
                NewCount = countsdetails.NewCount,
                PendingCount = countsdetails.PendingCount,
                ActiveCount = countsdetails.ActiveCount,
                ToClosedCount = countsdetails.ToClosedCount,
                ConcludeCount = countsdetails.ConcludeCount,
                UnpaidCount = countsdetails.UnpaidCount,
            };
            return Json(counts);
        }
        public IActionResult GetStatusCountsProvider(int id)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Expire Please Login!";
                return RedirectToAction("Index", "Login");
            }
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            if (physician == null)
            {
                TempData["Error"] = "Something Went Wrong Try Leter!!";
                return RedirectToAction("Index", "Login");
            }
            var countsdetails = _admin.GetStatusCountsAsync(physician.Physicianid);
            var counts = new
            {
                NewCount = countsdetails.NewCount,
                PendingCount = countsdetails.PendingCount,
                ActiveCount = countsdetails.ActiveCount,
                ConcludeCount = countsdetails.ConcludeCount
            };
            return Json(counts);
        }
        #endregion

        #region GetPhysician
        public IActionResult GetPhysician(string region)
        {
            try
            {
                List<Physician> physicians = _admin.GetPhysiciansByRegion(region);
                return Ok(physicians);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion


        #region ViewUploadsPage

        #region ViewUploads
        [CustomAuthorize("Dashboard", "6", "19")]
        [HttpGet("Admin/viewuploads/{id}", Name = "AdminViewUpload")]
        [HttpGet("Provider/viewuploads/{id}", Name = "ProviderUpload")]
        public async Task<IActionResult> ViewUploads(int id)
        {
            //Here Id Is Request Id
            //In This Page Show All Doucment Uploaded By Admin ,Physician And (User:-In Conclude State)
            string? email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                TempData["Error"] = "Session Is Expire Please Login!";
                return RedirectToAction("Index", "Login");
            }
            Physician? physician = _admin.GetPhysicianByEmail(email);
            Admin? admin = _admin.GetAdminByEmail(email);
            Requestclient? request = _admin.GetRequestclientByRequestId(id);
            //Check The Email Is Exist Or Not 
            if (request == null)
            {
                TempData["Error"] = "Something Went Wrong";
                if (admin != null)
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Provider");
            }
            if (admin != null)
            {
                ViewBag.IsPhysician = false;
            }
            if (physician != null)
            {
                if (request.Request.Physicianid != physician.Physicianid)
                {
                    TempData["Error"] = "Something Went Wrong";
                    return RedirectToAction("Index", "Provider");

                }
                ViewBag.IsPhysician = true;
            }
            //Get All File That Are Uploaded
            List<Requestwisefile> reqfile = await _patient.GetRequestwisefileByIdAsync(id);
            List<Requestwisefile> reqfiledeleted = reqfile.Where(item => item.Isdeleted != null && (item.Isdeleted.Length == 0 || !item.Isdeleted[0])).ToList();
            Request? requestclient = _admin.GetRequestById(id);
            RequestFileViewModel requestwiseviewmodel = new RequestFileViewModel
            {
                Request = requestclient,
                Requestid = id,
                Requestwisefileview = reqfiledeleted
            };
            return View(requestwiseviewmodel);
        }
        #endregion

        #region DeleteFile
        [HttpPost]
        public IActionResult DeleteFile(string filename)
        {
            try
            {
                Requestwisefile? file = _admin.GetRequestwisefileByFileName(filename);
                if (file != null)
                {

                    file.Isdeleted = new BitArray(new[] { true });
                    _admin.UpdateRequestWiseFile(file);
                    _admin.SaveChanges();
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
        #endregion

        #region DeleteSelectedFiles
        [HttpPost]
        public IActionResult DeleteSelectedFiles(List<string> filenames)
        {
            var url = Request.GetDisplayUrl;
            try
            {
                foreach (string filename in filenames)
                {
                    Requestwisefile? file = _admin.GetRequestwisefileByFileName(filename);

                    if (file != null)
                    {
                        file.Isdeleted = new BitArray(new[] { true });
                        _admin.UpdateRequestWiseFile(file);
                        _admin.SaveChanges();
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
        #endregion

        #region UploadFile
        [CustomAuthorize("Dashboard", "6", "19")]
        [HttpPost]
        public async Task<IActionResult> UploadFile(RequestFileViewModel rm, int id)
        {
            string? email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                TempData["Error"] = "Session Is Expire Please Login!";
                return RedirectToAction("Index", "Login");
            }
            Physician? physician = _admin.GetPhysicianByEmail(email);
            Admin? admin = _admin.GetAdminByEmail(email);
            if (rm.File != null)
            {
                if (physician != null)
                {
                    String? uniqueFileName = await _patient.AddFileInUploader(rm.File);
                    Requestwisefile requestWiseFile = new Requestwisefile();
                    {
                        requestWiseFile.Filename = uniqueFileName;
                        requestWiseFile.Createddate = DateTime.Now;
                        requestWiseFile.Requestid = id;
                        requestWiseFile.Isdeleted = new BitArray(new[] { false });
                        requestWiseFile.Physicianid = physician.Physicianid;
                    }
                    _admin.AddRequestWiseFile(requestWiseFile);
                    _admin.SaveChanges();
                    return RedirectToAction("ViewUploads", "Provider", new { id = id });
                }
                else
                {
                    String? uniqueFileName = await _patient.AddFileInUploader(rm.File);
                    Requestwisefile requestWiseFile = new Requestwisefile();
                    {
                        requestWiseFile.Filename = uniqueFileName;
                        requestWiseFile.Createddate = DateTime.Now;
                        requestWiseFile.Requestid = id;
                        requestWiseFile.Isdeleted = new BitArray(new[] { false });
                        requestWiseFile.Adminid = admin.Adminid;
                    }
                    _admin.AddRequestWiseFile(requestWiseFile);
                    _admin.SaveChanges();
                    return RedirectToAction("ViewUploads", "Admin", new { id = id });
                }
            }
            else
            {
                if (physician != null)
                    return RedirectToAction("ViewUploads", "Provider", new { id = id });
                else
                    return RedirectToAction("ViewUploads", "Admin", new { id = id });
            }
        }
        #endregion

        public async Task<IActionResult> ProviderUploadedDoc(IFormFile file, int requestId)
        {
            string? email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                TempData["Error"] = "Session Has Expired. Please Log In Again!";
                // Return "Bed Request" message
                return Ok("Bed Request");
            }

            Physician? physician = _admin.GetPhysicianByEmail(email);

            if (file != null)
            {
                string? uniqueFileName = await _patient.AddFileInUploader(file);
                var requestWiseFile = new Requestwisefile();
                {
                    requestWiseFile.Filename = uniqueFileName;
                    requestWiseFile.Createddate = DateTime.Now;
                    requestWiseFile.Requestid = requestId;
                    requestWiseFile.Isdeleted = new BitArray(new[] { false });
                    requestWiseFile.Physicianid = physician?.Physicianid;
                }
                _admin.AddRequestWiseFile(requestWiseFile);
                _admin.SaveChanges();
                TempData["SuccessMessage"] = "File Uploaded SuccessFully!!";
                return Ok();

            }
            else
            {
                // Return a specific message for the "else" condition
                TempData["Error"] = "File Uploaded UnSuccessFully!!";
                return Ok();
            }
        }


        #region SendEmailWithSelectedFiles
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
        #endregion

        #endregion

        #region OrderPage

        #region GetOrder
        [CustomAuthorize("SendOrder", "12", "21")]
        [HttpGet("Admin/SendOrder", Name = "AdminOrder")]
        [HttpGet("Provider/SendOrder", Name = "ProviderOrder")]
        public IActionResult SendOrder(int requestid)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            if (admin != null)
            {
                ViewBag.IsPhysician = false;
            }
            else
            {
                ViewBag.IsPhysician = true;
            }
            SendOrderModel sendOrderModel = _admin.GetSendOrder(requestid);

            return View(sendOrderModel);
        }
        #endregion

        #region SendOrder
        [CustomAuthorize("SendOrder", "12", "21")]
        [HttpPost]
        public IActionResult SendOrder(SendOrderModel order)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);

            bool result = _admin.SendOrders(order);
            if (result)
            {
                TempData["SuccessMessage"] = "Order successfully";
            }
            else
            {
                TempData["Error"] = "Order Unsuccessfully";
            }
            if (admin != null)
                return RedirectToAction("SendOrder", "Admin", new { requestid = order.requestid });
            else
                return RedirectToAction("SendOrder", "Provider", new { requestid = order.requestid });
        }
        #endregion

        #endregion

        public IActionResult VendorNameByHelthProfession(int helthprofessionaltype)
        {
            List<Healthprofessional> vendorname = _admin.GetHealthProfessionalByHealthProfessionalId(helthprofessionaltype);
            return Ok(vendorname);
        }

        public IActionResult BusinessDetails(int vendorname)
        {
            Healthprofessional? businessdetails = _admin.GetHealthprofessionalById(vendorname);
            return Ok(businessdetails);
        }

        #region SendAgreement
        [HttpPost]
        public IActionResult SendAgreement(int requestid, string agreementemail, string agreementphoneno)
        {
            //In This Page Will Show In the Send Agreement Of the Request I
            Request? request = _admin.GetRequestById(requestid);
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["SuccessMessage"] = "Session Is Not Found";
                return StatusCode(500, new { message = "Session Is Not Found" });
            }

            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            var protector = _dataProtectionProvider.CreateProtector("munavvar");
            string to = agreementemail;
            string requestidto = protector.Protect(requestid.ToString());
            string? Agreemnet = Url.Action("ReviewAgreement", "Request", new { requestid = requestidto }, protocol: HttpContext.Request.Scheme);
            string body = $"Click <a href='{Agreemnet}'>here</a> to Accept The Agreement";
            string subject = "Accept Agreement";
            if (admin != null)
            {

                if (_login.IsSendEmail(to, subject, body))
                {
                    _emailService.EmailLog(to, body, subject, request.Firstname + " " + request.Lastname, admin.Roleid, requestid, admin.Adminid, 0, 0, true, 1);
                    TempData["SuccessMessage"] = "Agreement Send successfully";
                    return Ok(new { Message = "send a mail", id = requestid, IsPhysician = false });
                }
            }
            else
            {
                if (_login.IsSendEmail(to, subject, body))
                {
                    _emailService.EmailLog(to, body, subject, request.Firstname + " " + request.Lastname, physician.Roleid, requestid, physician.Physicianid, 0, 0, true, 1);
                    TempData["SuccessMessage"] = "Agreement Send successfully";
                    return Ok(new { Message = "send a mail", id = requestid, IsPhysician = true });
                }
                else
                {
                    TempData["Error"] = "Agreement Send Unsuccessfully";
                    return Ok(new { Message = "send a mail", id = requestid, IsPhysician = true });
                }
            }
            return Json(false);
        }
        #endregion

        #region ClearCasePost
        [HttpPost]
        public IActionResult ClearCase(int requestidclearcase)
        {
            //Clear Case Is The Modal That Will Used To Clear The Case It Will Not Show The Dashboard It Will Closed The case Without Any Further Treetment
            try
            {
                string? email = HttpContext.Session.GetString("Email");
                int? id = HttpContext.Session.GetInt32("id");
                if (email == null)
                {
                    TempData["Error"] = "Session Is Expire Please Login!";
                    return RedirectToAction("Index", "Login");
                }
                Admin? admin = _admin.GetAdminByEmail(email);
                Request? request = _admin.GetRequestById(requestidclearcase);
                if (request != null)
                {
                    request.Status = 10;
                    _admin.UpdateRequest(request);
                    _admin.SaveChanges();

                    Requeststatuslog requeststatuslog = new Requeststatuslog();
                    requeststatuslog.Status = 10;
                    requeststatuslog.Requestid = requestidclearcase;
                    requeststatuslog.Createddate = DateTime.Now;
                    requeststatuslog.Adminid = admin?.Adminid;
                    _admin.AddRequestStatusLog(requeststatuslog);
                    _admin.SaveChanges();
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
        #endregion

        #region GeneratePDF
        [HttpGet("Admin/GeneratePDF", Name = "AdminGeneratePDF")]
        [HttpGet("Provider/GeneratePDF", Name = "ProviderGeneratePDF")]
        public IActionResult GeneratePDF(int requestid)
        {
            //Generate Pdf Is Used When  Physician Is Finalize The Encounter Form After That They Will Able To Download The Encounter Form 
            ViewEncounterForm viewEncounterForm = _admin.GetEncounterForm(requestid);

            if (viewEncounterForm == null)
            {
                return NotFound();
            }
            //For That It Used Rotativa EncounterFormDetails Is View That Map The Relation
            return new ViewAsPdf("EncounterFormDetails", viewEncounterForm)
            {
                FileName = "Encounter_Form.pdf"
            };

        }
        #endregion

        #region EncounterForm
        [HttpGet("Admin/EncounterForm", Name = "AdminEncounterForm")]
        [HttpGet("Provider/EncounterForm", Name = "ProviderEncounterForm")]
        public IActionResult EncounterForm(int requestId)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Not Found Please LogedIn Again!!";
                return RedirectToAction("Index", "Login");

            }
            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            Encounterform? encounterFormByRequestId = _admin.GetEncounteFormByRequestId(requestId);


            if (admin != null)
                ViewBag.IsPhysician = false;
            if (physician != null)
            {
                if (encounterFormByRequestId != null && encounterFormByRequestId.Request.Physicianid != physician.Physicianid)
                {
                    TempData["Error"] = "Something Went Wrong";
                    return RedirectToAction("Index", "Provider");
                }
                else if (encounterFormByRequestId != null && encounterFormByRequestId.IsFinalize && encounterFormByRequestId.Request.Physicianid == physician.Physicianid)
                {
                    TempData["Error"] = "Something Went Wrong";
                    return RedirectToAction("Index", "Provider");
                }
                ViewBag.IsPhysician = true;
            }
            ViewEncounterForm viewEncounterForm = new ViewEncounterForm();

            if (encounterFormByRequestId != null)
            {
                viewEncounterForm = _admin.GetEncounterForm(requestId);
                return View(viewEncounterForm);
            }
            else
            {
                Requestclient? request = _admin.GetRequestclientByRequestId(requestId);

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
                    TempData["Error"] = "Something Went Wrong";

                    if (admin != null)
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "Provider");
                }

                return View(viewEncounterForm);
            }
        }
        #endregion

        #region EncounterFormPost
        [HttpPost]
        public IActionResult EncounterForm(ViewEncounterForm viewEncounterForm, string requestid)
        {
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["SuccessMessage"] = "Session Is Not Found";
                return StatusCode(500, new { message = "Session Is Not Found" });
            }

            Admin? admin = _admin.GetAdminByEmail(Email);
            Physician? physician = _admin.GetPhysicianByEmail(Email);

            if (viewEncounterForm.IsFinalizied == "0")
            {
                _admin.SaveOrUpdateEncounterForm(viewEncounterForm, requestid);
                if (admin != null)
                    return RedirectToAction("EncounterForm", "Admin", new { requestId = requestid });
                else
                    return RedirectToAction("EncounterForm", "Provider", new { requestId = requestid });
            }
            else
            {
                _admin.SaveOrUpdateEncounterForm(viewEncounterForm, requestid);
                Encounterform? encounter = _admin.GetEncounteFormByRequestId(int.Parse(requestid));
                encounter.IsFinalize = true;
                _admin.UpdateEncounterForm(encounter);
                _admin.SaveChanges();
                TempData["SuccessMessage"] = "Encounter Form Finalize SuccessFully!!";

                if (admin != null)
                    return RedirectToAction("Index", "Admin", new { requestId = requestid });
                else
                    return RedirectToAction("Index", "Provider", new { requestId = requestid });
            }
        }
        #endregion

        #region CloseCase
        [HttpGet("Admin/CloseCase/", Name = "AdminCloseCase")]
        [HttpGet("Provider/ConcludeCare/", Name = "ProviderConcludeCare")]
        public async Task<IActionResult> CloseCase(int requestid)
        {
            //close and conclude care have same page it open Admin in to-close state
            //In Physician It open In conclude-state 
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Expire Please Login!";
                return RedirectToAction("Index", "Login");
            }
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            Admin? admin = _admin.GetAdminByEmail(Email);

            Requestclient? requestclient = _admin.GetRequestclientByRequestId(requestid);
            List<Requestwisefile> requestwisedocument = await _patient.GetRequestwisefileByIdAsync(requestid);
            if (requestclient == null)
            {

                TempData["Error"] = "Something Went Wrong";
                if (admin != null)
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Provider");
            }

            if (admin != null)
            {
                ViewBag.IsPhysician = false;
            }
            if (physician != null)
            {
                ViewBag.IsPhysician = true;
                if (requestclient.Request.Physicianid != physician.Physicianid)
                {
                    TempData["Error"] = "Dont Not Acess Other Request";
                    return RedirectToAction("Index", "Provider");
                }

            }
            //This Is Visible ONly In Admin Beacuse In Physician There Are Only Physician  Notes 


            string? requestclientnumber = requestclient.Request.Confirmationnumber;
            CloseCaseVM closecase = new CloseCaseVM();
            closecase.FirstName = requestclient?.Firstname ?? "";
            closecase.LastName = requestclient?.Lastname ?? "";
            closecase.Email = requestclient?.Email ?? "";
            closecase.PhoneNo = requestclient?.Phonenumber ?? "";
            closecase.BirthDate = requestclient != null && requestclient.Intyear != null && requestclient.Strmonth != null && requestclient.Intdate != null
                                  ? new DateOnly((int)requestclient.Intyear, int.Parse(requestclient.Strmonth), (int)requestclient.Intdate)
                                  : new DateOnly();
            closecase.Requestwisefileview = requestwisedocument;
            closecase.ConfirmNumber = requestclientnumber;
            closecase.Requestid = requestid;
            return View(closecase);


        }
        #endregion

        #region CloseCasePost
        [HttpPost]
        public IActionResult CloseCase(CloseCaseVM closeCaseVM, int requestid)
        {

            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                TempData["Error"] = "Session Is Expire Please Login!";
                return RedirectToAction("Index", "Login");
            }
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            Admin? admin = _admin.GetAdminByEmail(Email);

            if (!ModelState.IsValid)
            {
                return View(closeCaseVM);
            }
            else
            {
                Requeststatuslog requeststatuslog = new Requeststatuslog();
                //Check If Admin Is Not Null Then It Will Close State In Admin Dashboard
                //Else Its Conclude Care In Physician Dashboard
                if (admin != null)
                {

                    requeststatuslog.Adminid = admin.Adminid;
                    Requestclient? requestclient = _admin.GetRequestclientByRequestId(requestid);
                    if (requestclient != null)
                    {

                        requestclient.Phonenumber = closeCaseVM.PhoneNo;
                        requestclient.Email = closeCaseVM.Email;
                        _admin.UpdateRequestClientDataBase(requestclient);
                        _admin.SaveChanges();

                    }
                    return RedirectToAction("CloseCase", "Admin", new { requestid = requestid });
                }
                //Conclude Care In Physician Dashboard
                else
                {
                    Requestnote? requestnote = _admin.GetRequestNotesByRequestId(requestid);
                    requeststatuslog.Physicianid = physician.Physicianid;
                    //If Already Exsits Then It Will Be Update The Note
                    if (requestnote != null)
                    {
                        requestnote.Physiciannotes = closeCaseVM.Notes;
                        requestnote.Modifiedby = physician.Aspnetuserid;
                        requestnote.Modifieddate = DateTime.Now;
                        _admin.UpdateRequestNotes(requestnote);
                        _admin.SaveChanges();
                    }
                    else
                    {
                        Requestnote requestnote1 = new Requestnote();
                        requestnote1.Requestid = requestid;
                        requestnote1.Createdby = physician.Aspnetuserid;
                        requestnote1.Createddate = DateTime.Now;
                        requestnote1.Physiciannotes = closeCaseVM.Notes;
                        _admin.AddRequestNotes(requestnote1);
                        _admin.SaveChanges();
                    }
                    Request? request = _admin.GetRequestById(requestid);
                    if (request == null)
                    {
                        TempData["Error"] = "Request Is Not Found!!";
                        return RedirectToAction("ConcludeCare", "Provider", new { requestid = requestid });
                    }
                    request.Status = 8;
                    request.Modifieddate = DateTime.Now;
                    _admin.UpdateRequest(request);
                    _admin.SaveChanges();

                    requeststatuslog.Requestid = requestid;
                    requeststatuslog.Status = 8;
                    requeststatuslog.Createddate = DateTime.Now;
                    requeststatuslog.Notes = closeCaseVM.Notes;
                    _admin.AddRequestStatusLog(requeststatuslog);
                    _admin.SaveChanges();
                    TempData["SuccessMessage"] = "Conclude Care Is SuccessFully!!";

                    return RedirectToAction("Index", "Provider");

                }
            }
        }
        #endregion

        #region CheckEncounterFormFinalized
        public IActionResult CheckEncounterFormFinalized(int requestId)
        {
            //This Method Used When Physician Click On Conclude Care
            //It Checked The Encoutner Form Is Finalized Or Not If Encounter Form Is Not Finalized Show Sweet Alert

            Encounterform? encounterform = _admin.GetEncounteFormByRequestId(requestId);
            if (encounterform != null)
            {
                if (encounterform.IsFinalize)
                {
                    return Ok("Finalized");

                }
                else
                {
                    return Ok("NotFinalize");
                }
            }
            else
            {
                return Ok("NotFinalize");

            }
        }
        #endregion

        #endregion

        #region GrantAccessOfEdit
        public IActionResult GrantAccessOfEdit(int id)
        {
            //GrantAccessOfEditPhysicianAccount
            Physician physician = _admin.GetPhysicianById(id);
            physician.Iscredentialdoc = new BitArray(new[] { true });
            _admin.UpdatePhysicianDataBase(physician);
            _admin.SaveChanges();
            TempData["SuccessMessage"] = "Creadintial Gived Successfully";
            return RedirectToAction("Index", "Login");
        }
        #endregion

        private bool IsAnyBitSet(BitArray bitArray)
        {
            foreach (bool bit in bitArray)
            {
                if (!bit)
                {
                    return true; // If any bit is false, return true
                }
            }
            return false; // If all bits are true, return false
        }


        public IActionResult Payrate(int physicianId)
        {
            var PayrateDetails = _admin.GetPayRateDetails(physicianId);
            return View(PayrateDetails);
        }

        public IActionResult EditPayrate(ViewProviderPayrate viewProviderPayrate)
        {
            string email = HttpContext.Session.GetString("Email");
            Admin admin = _admin.GetAdminByEmail(email);
            var EditPayrate = _admin.EditPayRate((int)viewProviderPayrate.ProviderPayrateId, (decimal)viewProviderPayrate.Payrate, admin.Aspnetuserid);
            return RedirectToAction("Payrate", "Admin", new { physicianId = viewProviderPayrate.PhysicianId });
        }

        [HttpGet("Provider/Invoice", Name = "ProviderInvoice")]
        [HttpGet("Admin/Invoice", Name = "AdminInvoice")]
        public IActionResult Invoice()
        {
            string email = HttpContext.Session.GetString("Email");
            Admin? admin = _admin.GetAdminByEmail(email);
            Physician physician = _admin.GetPhysicianByEmail(email);
            string region="";
            ViewBag.ProviderComboBox = _admin.GetAllPhysician();
            if (admin != null)
                ViewBag.IsPhysician = false;
            else if (physician != null)
                ViewBag.IsPhysician = true;
            ViewData["ViewName"] = "Invoicing";
            return View();
        }

        public IActionResult IsFinalizeSheet(DateOnly startDate)
        {
            string email = HttpContext.Session.GetString("Email");
            Physician physician = _admin.GetPhysicianByEmail(email);

            if (physician != null)
            {
                bool x = _invoiceInterface.isFinalizeTimesheet((int)physician.Physicianid, startDate);
                return Json(new { x });
            }
            else
            {
                return BadRequest();
            }

        }
        public IActionResult IsFinalizeSheetAdmin(int PhysicianId,DateOnly startDate)
        {
                bool x = _invoiceInterface.isFinalizeTimesheet(PhysicianId, startDate);
                return Json(new { x });
           

        }
        [HttpGet("Provider/TimeSheet", Name = "ProviderTimeSheet")]

        public IActionResult TimeSheet(DateOnly startDate)
        {
            ViewData["ViewName"] = "Invoicing";
            string? email = HttpContext.Session.GetString("Email");
            Physician? physician=_admin.GetPhysicianByEmail(email);
            
            if (physician.Physicianid != null && _invoiceInterface.isFinalizeTimesheet((int)physician.Physicianid, startDate))
            {
                TempData["error"] = "Sheet is already Finalized";
                return RedirectToAction("Invoicing", "ProviderSite");
            }

            else if (physician.Physicianid != null && physician.Aspnetuserid != null)
            {
                int afterDays = startDate.Day == 1 ? 14 : DateTime.DaysInMonth(startDate.Year, startDate.Month) - 14; ;
                var TimesheetDetails = _invoiceInterface.PostTimesheetDetails((int)physician.Physicianid, startDate, afterDays, physician.Aspnetuserid);
                List<TimesheetDetailReimbursement> h = _invoiceInterface.GetTimesheetBills(TimesheetDetails);
                var Timesheet = _invoiceInterface.GetTimesheetDetails(TimesheetDetails, h, (int)physician.Physicianid);
                Timesheet.PhysicianId = (int)physician.Physicianid;
                Timesheet.TimeSheetDetails[0].StartDate = startDate;
                ViewBag.IsPhysician = false;
                return View("Timesheet", Timesheet);
            }
            else
            {
                return BadRequest();
            }
        }
        public IActionResult GetTimesheetDetails(DateOnly StartDate)
        {
            string? email = HttpContext.Session.GetString("Email");
            Physician physician = _admin.GetPhysicianByEmail(email);
            Admin admin = _admin.GetAdminByEmail(email);
            if (physician.Physicianid != null && physician.Aspnetuserid != null && StartDate != DateOnly.MinValue)
            {
                List<TimesheetDetail>? x = _invoiceInterface.PostTimesheetDetails(physician.Physicianid, StartDate, 0, physician.Aspnetuserid);
                List<TimesheetDetailReimbursement>? h = _invoiceInterface.GetTimesheetBills(x);
                ViewTimeSheet timeSheet = _invoiceInterface.GetTimesheetDetails(x, h, physician.Physicianid);
                return PartialView("_TimesheetDetailTable", timeSheet);
            }
            
            else
            {
                ViewTimeSheet timeSheet = new ViewTimeSheet()
                {
                    TimeSheetDetails = new List<ViewTimeSheetDetails>(),
                    TimeSheetDetailReimbursements = new List<TimeSheetDetailReimbursements>(),
                    PayrateWithProvider = new List<PayrateByProvider>(),
                };
                return PartialView("_TimesheetDetailTable", timeSheet);
            }
        }
        [HttpPost]
        public IActionResult TimeSheetDetailsEdit(ViewTimeSheet viewTimeSheet)
        {
            string? email = HttpContext.Session.GetString("Email");
            Physician physician = _admin.GetPhysicianByEmail(email);

            if (_invoiceInterface.PutTimesheetDetails(viewTimeSheet.TimeSheetDetails, physician.Aspnetuserid))
            {
                TempData["success"] = ("Timesheet edited Successfully..!");
            }

            return RedirectToAction("Timesheet", new { StartDate = viewTimeSheet.TimeSheetDetails[0].StartDate});
        }
        public IActionResult TimeSheetBillAddEdit(int? Trid, DateOnly Timesheetdate, IFormFile file, int Timesheetdetailid, int Amount, string Item, int PhysicianId, DateOnly StartDate)
        {
            string? email = HttpContext.Session.GetString("Email");
            Physician physician = _admin.GetPhysicianByEmail(email);

            TimeSheetDetailReimbursements timesheetdetailreimbursement = new TimeSheetDetailReimbursements();
            timesheetdetailreimbursement.Timesheetdetailid = Timesheetdetailid;
            timesheetdetailreimbursement.Timesheetdetailreimbursementid = Trid;
            timesheetdetailreimbursement.Amount = Amount;
            timesheetdetailreimbursement.BillFile = file;
            timesheetdetailreimbursement.Itemname = Item;
            
            if (_invoiceInterface.TimeSheetBillAddEdit(timesheetdetailreimbursement,physician.Aspnetuserid ))
            {
                TempData["success"] = ("Bill Changed Succesfull..!");
            }
            return RedirectToAction("Timesheet", new { PhysicianId = PhysicianId, StartDate = StartDate });
        }
        #region TimeSheetBill_Delete
        public IActionResult TimeSheetBillRemove(int? Trid, int PhysicianId, DateOnly StartDate)
        {
            string? email = HttpContext.Session.GetString("Email");
            Physician physician = _admin.GetPhysicianByEmail(email);

            TimeSheetDetailReimbursements timesheetdetailreimbursement = new TimeSheetDetailReimbursements();
            timesheetdetailreimbursement.Timesheetdetailreimbursementid = Trid;
            if (_invoiceInterface.TimeSheetBillRemove(timesheetdetailreimbursement, physician.Aspnetuserid))
            {
                TempData["success"] = ("Bill Deleted Succesfull..!");

            }
            return RedirectToAction("Timesheet", new { PhysicianId = PhysicianId, StartDate = StartDate });
        }
        #endregion
       
        public IActionResult SetToFinalize(int timesheetid)
        {
            string? email = HttpContext.Session.GetString("Email");
            Physician? physician = _admin.GetPhysicianByEmail(email);

            if (_invoiceInterface.SetToFinalize(timesheetid, physician.Aspnetuserid))
            {
                TempData["success"] = ("Bill Deleted Succesfull..!");
            }
            return RedirectToAction("Index","Provider");
        }
        public IActionResult IndexAdmin()
        {
            ViewBag.GetAllPhysicians = _admin.GetAllPhysician();
            return View();
        }

        public IActionResult IsApproveSheet(int PhysicianId, DateOnly StartDate)
        {
            var x = _invoiceInterface.GetPendingTimesheet(PhysicianId, StartDate);
            if (x.Count() == 0)
            {
                return Json(new { x = true });
            }
            return PartialView("_PendingApprove", x);
        }
        #region ChatPerson
        public IActionResult ChatPerSonDetails(int physicianId)
        {
            Physician physician=_admin.GetPhysicianById(physicianId);
            return Json(physician);
        }

        public async Task<IActionResult> GetTimesheetDetailsData(int PhysicianId, DateOnly StartDate)
        {
            var Timesheet = new ViewTimeSheet();
            string Email = HttpContext.Session.GetString("Email");
            Admin admin = _admin.GetAdminByEmail(Email);
            if (StartDate == DateOnly.MinValue || PhysicianId == 0)
            {
                Timesheet.TimeSheetDetails = new List<ViewTimeSheetDetails> { };
                Timesheet.TimeSheetDetailReimbursements = new List<TimeSheetDetailReimbursements> { };
            }
            else
            {
                List<TimesheetDetail> x = _invoiceInterface.PostTimesheetDetails(PhysicianId, StartDate, 0, admin.Aspnetuserid);
                List<TimesheetDetailReimbursement> h =  _invoiceInterface.GetTimesheetBills(x);
                Timesheet = _invoiceInterface.GetTimesheetDetails(x, h, PhysicianId);
            }
            if (Timesheet == null)
            {
                var Timesheets = new ViewTimeSheet();
                Timesheets.TimeSheetDetails = new List<ViewTimeSheetDetails> { };
                Timesheets.TimeSheetDetailReimbursements = new List<TimeSheetDetailReimbursements> { };
                return PartialView("_TimesheetDetailTable", Timesheets);
            }


            return PartialView("_TimesheetDetailTable", Timesheet);
        }
        #endregion
    }
}
