using BusinessLayer.InterFace;
using CsvHelper;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rotativa.AspNetCore;
using ServiceStack;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web.Helpers;
using BC = BCrypt.Net.BCrypt;

namespace HelloDoc.Controllers
{
	public class AdminController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IAdmin _admin;
		private readonly IPatientRequest _patient;
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly IJwtAuth _jwtAuth;
		private readonly ILogin _login;
		private readonly IUploadProvider _uploadProvider;
		private readonly IDataProtectionProvider _dataProtectionProvider;

		public AdminController(ApplicationDbContext context, IUploadProvider uploadProvider, IAdmin admin, IPatientRequest patient, IDataProtectionProvider dataProtectionProvider, IWebHostEnvironment hostingEnvironment, IJwtAuth jwtAuth, ILogin login)
		{
			_context = context;
			_admin = admin;
			_uploadProvider = uploadProvider;
			_patient = patient;
			_jwtAuth = jwtAuth;
			_hostingEnvironment = hostingEnvironment;
			_login = login;
			_dataProtectionProvider = dataProtectionProvider;
		}
		//main View
		[CustomAuthorize("ProviderLocation", "17")]
		public IActionResult ProviderLocation()
		{
			return View();
		}
		[CustomAuthorize("ProviderLocation", "17")]
		public List<PhysicianLocation> GetProviders()
		{
			return _context.PhysicianLocations.ToList();
		}

		#region Profile
		[CustomAuthorize("MyProfile", "5")]
		public IActionResult Profile()
		{
			string? email = HttpContext.Session.GetString("Email");

			if (string.IsNullOrEmpty(email))
			{
				TempData["Error"] = "Invalid session data. Please log in again.";
				return RedirectToAction("Index", "Admin");
			}

			var adminProfile = _admin.GetAdminProfile(email);

			if (adminProfile == null)
			{
				TempData["Error"] = "Something went wrong while fetching admin profile.";
				return RedirectToAction("Index", "Admin");
			}

			return View(adminProfile);
		}
		#endregion

		#region ProfileByID
		[CustomAuthorize("MyProfile", "5")]
		public IActionResult EditProfile(int adminid)
		{
			if (adminid == 0)
			{
				TempData["Error"] = "Admin Value is Not Match";
				return RedirectToAction("Index", "Admin");

			}
			string? email = _context.Admins.Where(item => item.Adminid == adminid).Select(item => item.Email).FirstOrDefault();
			if (string.IsNullOrEmpty(email))
			{
				TempData["Error"] = "Something Went Wrong.";
				return RedirectToAction("Index", "Admin");

			}

			var adminProfile = _admin.GetAdminProfile(email);

			if (adminProfile == null)
			{
				TempData["Error"] = "Something went wrong while fetching admin profile.";
				return RedirectToAction("Index", "Admin");
			}
			return View("Profile", adminProfile);

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
		public IActionResult AdministrationInfo(string email, string mobileNo, string[] adminRegion)
		{
			string? sessionEmail = HttpContext.Session.GetString("Email");

			if (string.IsNullOrEmpty(sessionEmail))
			{
				TempData["Error"] = "Invalid session data. Please log in again.";
				return RedirectToAction("Index", "Admin");
			}
			try
			{
				_admin.UpdateAdministrationInfo(sessionEmail, email, mobileNo, adminRegion);
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

		#region Provider
		[CustomAuthorize("Provider", "8")]
		public IActionResult Provider()
		{
			List<Region> region = _context.Regions.ToList();
			return View(region);
		}
		#endregion

		#region ProviderData
		[CustomAuthorize("Provider", "8")]
		public IActionResult ProviderData(string region)
		{
			var providers = _admin.GetProviders(region);
			return PartialView("ProviderProfileTablePartial", providers);
		}
		#endregion

		#region PhysicanProfile
		[CustomAuthorize("Provider", "8")]
		public IActionResult PhysicanProfile(int id)
		{
			try
			{
				var providerProfile = _admin.GetPhysicianProfile(id);
				return View(providerProfile);
			}
			catch (InvalidOperationException ex)
			{
				TempData["Error"] = ex.Message;
				return RedirectToAction("Index", "Error");
			}
		}
		#endregion

		#region ResetPhysicianPassword
		[CustomAuthorize("Provider", "8")]
		public IActionResult ResetPhysicianPassword(string password, int physicianId)
		{
			try
			{
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
			catch (InvalidOperationException ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction("PhysicanProfile", "Admin", new { id = physicianId });
		}
		#endregion

		#region SaveSignatureImage
		[CustomAuthorize("Provider", "8")]
		[HttpPost]
		public IActionResult SaveSignatureImage(IFormFile signatureImage, int id)
		{
			try
			{
				if (signatureImage != null && signatureImage.Length > 0)
				{
					string fileName = _uploadProvider.UploadSignature(signatureImage, id);
					var physician = _context.Physicians.FirstOrDefault(item => item.Physicianid == id);
					physician.Signature = fileName;
					_context.Physicians.Update(physician);
					_context.SaveChanges();
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
		[CustomAuthorize("Provider", "8")]
		[HttpPost]
		public IActionResult UploadDoc(string fileName, IFormFile File, int physicianid)
		{
			Physician? physician = _context.Physicians.FirstOrDefault(item => item.Physicianid == physicianid);
			if (physician != null)
			{

				if (fileName == "ICA")
				{
					var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
					physician.Isagreementdoc = new BitArray(new[] { true });
				}
				if (fileName == "Background")
				{
					var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
					physician.Isbackgrounddoc = new BitArray(new[] { true });
				}
				if (fileName == "Hippa")
				{
					var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
					physician.Istrainingdoc = new BitArray(new[] { true });
				}
				if (fileName == "NonDiscoluser")
				{
					var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
					physician.Isnondisclosuredoc = new BitArray(new[] { true });
				}
				if (fileName == "License")
				{
					var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
					physician.Islicensedoc = new BitArray(new[] { true });
				}
				_context.Physicians.Update(physician);
				_context.SaveChanges();
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
		[CustomAuthorize("Provider", "8")]
		[HttpPost]
		public IActionResult PhysicianInformation(string email, int id, string mobileNo, string[] adminRegion, string synchronizationEmail, string npinumber, string medicalLicense)
		{
			try
			{
				_admin.UpdatePhysicianInformation(id, email, mobileNo, adminRegion, synchronizationEmail, npinumber, medicalLicense);
				TempData["SuccessMessage"] = "Physician information has been updated successfully.";
			}
			catch (InvalidOperationException ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction("PhysicanProfile", "Admin", new { id = id });
		}
		#endregion

		#region Providerprofile
		[CustomAuthorize("Provider", "8")]
		[HttpPost]
		public IActionResult Providerprofile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile)
		{
			try
			{
				_admin.UpdateProviderProfile(id, businessName, businessWebsite, signatureFile, photoFile);
				TempData["SuccessMessage"] = "Provider profile has been updated successfully.";
			}
			catch (InvalidOperationException ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction("PhysicanProfile", "Admin", new { id = id });
		}
		#endregion

		#region ProviderAccountingInfo
		[CustomAuthorize("Provider", "8")]
		[HttpPost]
		public IActionResult ProviderAccountingInfo(int physicianid, string Address1, string Address2, string City, int State, string Zipcode, string MobileNo)
		{
			bool success = _admin.UpdatePhysicianAccountingInfo(physicianid, Address1, Address2, City, State, Zipcode, MobileNo);
			if (success)
			{
				TempData["SuccessMessage"] = "Provider AccountingInfo Saved.";
			}
			else
			{
				TempData["ErrorMessage"] = "Provider not found or invalid ID.";
			}
			return RedirectToAction("PhysicanProfile", "Admin", new { id = physicianid });
		}
		#endregion

		#region CreateProvider
		public IActionResult CreateProvider()
		{
			List<Region> regions = _admin.GetAllRegion();
			List<Role> roles = _context.Roles.Where(item => item.Accounttype == 2).ToList();
			List<Role> providerrole = roles.Where(item => item.Isdeleted[0]).ToList();
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

		#region CreateProvider
		[HttpPost]
		public IActionResult CreateProvider(CreateProviderVM createProvider, string[] adminRegion)
		{
			string? email = HttpContext.Session.GetString("Email");
			string? id = HttpContext.Session.GetString("aspnetid");
			Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);

			AspnetUser aspnetUser = new AspnetUser();
			aspnetUser.Username = createProvider.UserName;
			aspnetUser.Aspnetuserid = Guid.NewGuid().ToString();
			aspnetUser.Email = createProvider.Email;
			aspnetUser.Passwordhash = BC.HashPassword(createProvider.Passwordhash);
			aspnetUser.Phonenumber = createProvider.PhoneNo;
			_context.AspnetUsers.Add(aspnetUser);
			_context.SaveChanges();
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
			physician.Businessname = createProvider.BusinessName;
			physician.Businesswebsite = createProvider.BusinessWebsite;
			physician.Npinumber = createProvider.NPINumber;
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
			_context.Physicians.Add(physician);
			_context.SaveChanges();
			if (createProvider.File != null)
			{
				var photo = _uploadProvider.UploadPhoto(createProvider.File, physician.Physicianid);
			}
			if (createProvider.IsAgreement != null)
			{
				var ICA = _uploadProvider.UploadDocFile(createProvider.IsAgreement, physician.Physicianid, "ICA");
			}
			if (createProvider.IsBackground != null)
			{
				var ICA = _uploadProvider.UploadDocFile(createProvider.IsBackground, physician.Physicianid, "Background");
			}
			if (createProvider.IsHippa != null)
			{
				var ICA = _uploadProvider.UploadDocFile(createProvider.IsHippa, physician.Physicianid, "Hippa");
			}
			if (createProvider.License != null)
			{
				var ICA = _uploadProvider.UploadDocFile(createProvider.License, physician.Physicianid, "License");
			}
			if (createProvider.NonDiscoluser != null)
			{
				var ICA = _uploadProvider.UploadDocFile(createProvider.License, physician.Physicianid, "NonDiscoluser");
			}
			PhysicianNotification physicianNotification = new PhysicianNotification();
			physicianNotification.Physicianid = physician.Physicianid;
			physicianNotification.Isnotificationstopped = new BitArray(new[] { true });
			_context.PhysicianNotifications.Add(physicianNotification);
			_context.SaveChanges();
			foreach (var item in adminRegion)
			{

				PhysicianRegion physicianRegion = new PhysicianRegion();
				physicianRegion.Physicianid = physician.Physicianid;
				physicianRegion.Regionid = int.Parse(item);
				_context.PhysicianRegions.Add(physicianRegion);
			}
			_context.SaveChanges();

			AspnetUserrole aspnetUserrole = new AspnetUserrole();
			aspnetUserrole.Userid = aspnetUser.Aspnetuserid;
			aspnetUserrole.Roleid = createProvider.RoleId;
			_context.AspnetUserroles.Add(aspnetUserrole);
			_context.SaveChanges();

			return RedirectToAction("Provider");
		}
		#endregion

		#region Scheduling
		public IActionResult Scheduling()
		{
			var region = _admin.GetAllRegion();
			ViewBag.regions = region;
			return View();
		}
		#endregion

		#region CreateShift
		public IActionResult CreateShift(ScheduleModel data)
		{
			List<DateTime> ConflictingDates = _admin.IsShiftOverwritting(data);
			if (ConflictingDates.Count > 0)
			{
				TempData["Error"] = $"Conflicting shifts found on: {string.Join(", ", ConflictingDates.Select(d => d.ToString("dd-MM-yyyy")))}";
			}
			else
			{
				string email = HttpContext.Session.GetString("Email");
				Admin adminId = _admin.GetAdminByEmail(email);
				data.Status = 1;
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
			var model = _admin.GetProvidersOnCall(region);
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
			var events = _admin.GetEvents(region);
			var mappedEvents = events.Select(e => new
			{
				id = e.Shiftid,
				resourceId = e.Physicianid,
				title = e.PhysicianName,
				start = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Starttime.Hour, e.Starttime.Minute, e.Starttime.Second),
				end = new DateTime(e.Shiftdate.Value.Year, e.Shiftdate.Value.Month, e.Shiftdate.Value.Day, e.Endtime.Hour, e.Endtime.Minute, e.Endtime.Second),
				ShiftDetailId = e.ShiftDetailId,
				region = _context.Regions.Where(i => i.Regionid == e.Regionid),
				status = e.Status
			}).ToList();

			return Ok(mappedEvents);
		}
		#endregion

		#region SaveShift
		[HttpPost]
		public IActionResult SaveShift(int shiftDetailId, DateTime startDate, TimeOnly startTime, TimeOnly endTime, int region)
		{
			Shiftdetail? shiftdetail = _admin.GetShiftDetailById(shiftDetailId);

			if (shiftdetail == null)
			{
				return NotFound("Shift detail not found.");
			}
			try
			{
				shiftdetail.Shiftdate = startDate;
				shiftdetail.Starttime = startTime;
				shiftdetail.Endtime = endTime;

				_admin.UpdateShiftDetail(shiftdetail);
				_admin.SaveChanges();
				var events = _admin.GetEvents(region);
				var mappedEvents = events.Select(e => new
				{
					id = e.Shiftid,
					resourceId = e.Physicianid,
					title = e.PhysicianName,
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
			var events = _admin.GetEvents(region);
			var mappedEvents = events.Select(e => new
			{
				id = e.Shiftid,
				resourceId = e.Physicianid,
				title = e.PhysicianName,
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
			var events = _admin.GetEvents(region);
			var mappedEvents = events.Select(e => new
			{
				id = e.Shiftid,
				resourceId = e.Physicianid,
				title = e.PhysicianName,
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
				foreach (var id in selectedIds)
				{
					Shiftdetail shiftDetail = _admin.GetShiftDetailById(id);
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
				foreach (var id in selectedIds)
				{
					Shiftdetail shiftDetail = _admin.GetShiftDetailById(id);
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

		#region Records
		public IActionResult Records()
		{
			return View();
		}
		#endregion

		#region UpdateShiftDates
		public IActionResult UpdateShiftDates()
		{
			return Ok();
		}
		#endregion

		#region GetPatientHistoryData
		public IActionResult GetPatientHistoryData(string firstName, string lastName, string email, string phoneNumber, int page)
		{
			List<User> users = _admin.GetUsers(firstName, lastName, email, phoneNumber);
			List<PatientHistoryVM> patientHistoryVms = users.Select(user => new PatientHistoryVM
			{
				FirstName = user.Firstname,
				LastName = user.Lastname,
				Email = user.Email,
				PhoneNumber = user.Mobile,
				Address = user.City,
				UserId = user.Userid
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

		#region EditPartner
		public IActionResult EditPartner(int partnerId)
		{
			Healthprofessional? healthprofessional = _admin.GetHealthprofessionalById(partnerId);
			List<Healthprofessionaltype> healthprofessionaltype = _admin.GetAllHealthprofessoionalType();
			List<Region> Regions = _admin.GetAllRegion();
			ViewBag.Regions = Regions;
			ViewBag.helthprofessional = healthprofessionaltype;
			HealthProffesionalVM healthProffesionalVM = new HealthProffesionalVM();
			healthProffesionalVM.Businesscontact = healthprofessional.Businesscontact;
			healthProffesionalVM.Profession = (int)healthprofessional.Healthprofessionalid;
			healthProffesionalVM.Vendorname = healthprofessional.Vendorname;
			healthProffesionalVM.Faxnumber = healthprofessional.Faxnumber;
			healthProffesionalVM.Address = healthprofessional.Address;
			healthProffesionalVM.City = healthprofessional.City;
			healthProffesionalVM.State = healthprofessional.State;
			healthProffesionalVM.Zip = healthprofessional.Zip;
			healthProffesionalVM.Email = healthprofessional.Email;
			healthProffesionalVM.Phonenumber = healthprofessional.Phonenumber;
			healthProffesionalVM.HealthProfessionId = healthprofessional.Vendorid;
			return View(healthProffesionalVM);
		}
		#endregion

		#region EditPartner
		[HttpPost]
		public IActionResult EditPartner(HealthProffesionalVM healthProffesionalVM)
		{
			Healthprofessional? healthprofessional = _admin.GetHealthprofessionalById(healthProffesionalVM.HealthProfessionId);

			healthprofessional.Vendorname = healthProffesionalVM.Vendorname;
			healthprofessional.Healthprofessionalid = healthProffesionalVM.HealthProfessionId;
			healthprofessional.Email = healthProffesionalVM.Email;
			healthprofessional.Businesscontact = healthProffesionalVM.Businesscontact;
			healthprofessional.Address = healthProffesionalVM.Address;
			healthprofessional.Phonenumber = healthProffesionalVM.Phonenumber;
			healthprofessional.City = healthProffesionalVM.City;
			healthprofessional.State = healthProffesionalVM.State;
			healthprofessional.Regionid = _context.Regions.Where(r => r.Name == healthProffesionalVM.State).FirstOrDefault().Regionid;
			healthprofessional.Zip = healthProffesionalVM.Zip;
			healthprofessional.Faxnumber = healthProffesionalVM.Faxnumber;
			_admin.UpdateHealthPrifessional(healthprofessional);
			_admin.SaveChanges();
			return RedirectToAction("EditPartner", new { partnerId = healthprofessional.Vendorid });
		}
		#endregion

		#region PatientRecords
		public IActionResult PatientRecords(int userId)
		{
			List<PatientHistoryVM> patientRecords = _admin.GetPatientRecords(userId);
			return View(patientRecords);
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

		public DateTime? GetDateofService(int requestid)
		{
			Requeststatuslog? log = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).FirstOrDefault(x => x.Requestid == requestid && x.Status == 6 && x.Physicianid != null);
			return log?.Createddate;
		}

		public DateTime? GetCloseDate(int requestid)
		{
			Requeststatuslog? log = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).FirstOrDefault(x => x.Requestid == requestid && x.Status == 9);
			return log?.Createddate;
		}
		public string? GetPatientCancellationNotes(int requestid)
		{
			Requeststatuslog? log = _context.Requeststatuslogs.OrderByDescending(x => x.Createddate).FirstOrDefault(x => x.Requestid == requestid && x.Status == 3 && x.Physicianid != null);
			return log?.Notes;
		}

		#region GetPatientSearchRecords
		public IActionResult GetPatientSearchRecords(int[] status, string patientName, int requestType, string providorName, string email, string phoneNumber, bool exportStatus)
		{
			var data = (from r in _context.Requests
						join rc in _context.Requestclients on r.Requestid equals rc.Requestid
						join p in _context.Physicians on r.Physicianid equals p.Physicianid into prJoin
						from p in prJoin.DefaultIfEmpty()
						join rn in _context.Requestnotes on r.Requestid equals rn.Requestid into rrnJoin
						from rn in rrnJoin.DefaultIfEmpty()
						select new
						{
							Request = r,
							RequestClient = rc,
							Physician = p,
							RequestNote = rn
						}).ToList();

			var searchRecords = data.Select(item => new SearchRecordVM
			{
				PatientName = $"{item.RequestClient.Firstname} {item.RequestClient.Lastname}",
				Requestor = $"{item.Request.Firstname} {item.Request.Lastname}",
				DateOfService = GetDateofService(item.Request.Requestid),
				ServiceDate = GetDateofService(item.Request.Requestid)?.ToString("MMMM dd, yyyy") ?? "",
				DateofClose = GetCloseDate(item.Request.Requestid)?.ToString("MMMM dd, yyyy") ?? "",
				CloseDate = GetCloseDate(item.Request.Requestid),
				Email = item.RequestClient.Email,
				PhoneNumber = item.RequestClient.Phonenumber,
				Address = item.RequestClient.Location,
				Zip = item.RequestClient.Zipcode,
				RequestStatus = item.Request.Status,
				PhysicianName = item.Physician != null ? $"{item.Physician.Firstname} {item.Physician.Lastname}" : "", // Handle null Physician
				PhysicianNote = item.RequestNote?.Physiciannotes,
				CancelledByProvidor = GetPatientCancellationNotes(item.Request.Requestid),
				PatientNote = item.RequestClient.Notes,
				RequestTypeId = item.Request.Requesttypeid,
				AdminNotes = item.RequestNote?.Adminnotes,
				RequestId = item.Request.Requestid,
				IsDelted=item.Request.Isdeleted,
			}).ToList();
			 var searchRecord = searchRecords.Where(item =>
	   (string.IsNullOrEmpty(email) || item.Email.Contains(email)) &&
	   (string.IsNullOrEmpty(phoneNumber) || item.PhoneNumber.Contains(phoneNumber)) &&
	   (string.IsNullOrEmpty(patientName) || item.PatientName.ToLower().Contains(patientName)) &&
	   (string.IsNullOrEmpty(providorName) || item.PhysicianName.ToLower().Contains(providorName)) &&
	   (status.Length==0 || status.Contains(item.RequestStatus)) && item.IsDelted==false &&
	   (requestType == 0 || item.RequestTypeId == requestType)
	   //&&
	   //(fromdos == null || item.dateofservice >= fromdos) &&
	   //(todos == null || item.DateOfService <= ToDoS)
   ).ToList();
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

			return PartialView("SearchRecordsPartial", searchRecord);
		}
		#endregion


		public IActionResult DeletePatient(int id)
		{
			Request? request = _context.Requests.Find(id);
				if (request != null)
			{
				request.Isdeleted = true;
			_context.Requests.Update(request);
			}
			_admin.SaveChanges();
			return RedirectToAction("SearchRecords");
		}


		[CustomAuthorize("Role", "7")]
		public IActionResult Access()
		{
			List<Role> roles = _admin.GetAllRoles();
			var list = roles.Where(item => item.Isdeleted != null && (item.Isdeleted.Length == 0 || !item.Isdeleted[0]));
			return View(list.ToList());
		}

		public IActionResult BlockHistory()
		{
			return View();
		}

		public IActionResult GetBLockHistory(string name,string email ,string phonenumber)
		{
			var blockdata = _context.Blockrequests.Include(b => b.Request).Where(item => string.IsNullOrEmpty(name) || item.Request.Firstname.Contains(name)).ToList();
			return PartialView("BlockHistoryPartial", blockdata);
		}
		
		public IActionResult UnBlockUser(int id)
		{
			Blockrequest? blockHistory = _context.Blockrequests.Find(id);
			if (blockHistory != null)
			{
				_context.Remove(blockHistory);
				_context.SaveChanges();
			}
			return RedirectToAction("BlockHistory");
		}

		public IActionResult SearchRecords()
		{
			return View();
		}

		[CustomAuthorize("Role", "7")]
		public IActionResult CreateAccess()
		{
			return View();
		}

		#region CreateAccess
		[CustomAuthorize("Role", "7")]
		[HttpPost]
		public IActionResult CreateAccess(int[] rolemenu, string rolename, int accounttype)
		{
			Role role = new Role();
			role.Name = rolename;
			role.Accounttype = (short)accounttype;
			role.Createdby = "admin";
			role.Createddate = DateTime.Now;
			role.Isdeleted = new BitArray(new[] { false });
			_admin.AddRoles(role);
			_admin.SaveChanges();

			foreach (var menu in rolemenu)
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
			List<Menu> menuList = _admin.GetMenuByAccountType(region);
			return PartialView("accesspartial", menuList);
		}
		#endregion

		#region EditRoleData
		[CustomAuthorize("Role", "7")]
		public IActionResult EditRoleData(int region, int roleid)
		{
			List<Menu> menuList = _admin.GetMenuByAccountType(region);
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
			List<int> rolemenu = _admin.GetRoleMenuIdByRoleId(roleid);
			Role role = _admin.GetAllRolesById(roleid);
			AccessVM accessVM = new AccessVM();
			accessVM.Menu = rolemenu;
			accessVM.Name = role.Name;
			accessVM.roleid = roleid;
			accessVM.Accounttype = role.Accounttype;

			return View(accessVM);
		}
		#endregion

		#region EditAccess
		[CustomAuthorize("Role", "7")]
		[HttpPost]
		public IActionResult EditAccess(int roleid, int[] rolemenu, string rolename, int accounttype)
		{
			Role role = _admin.GetAllRolesById(roleid);
			List<Rolemenu> menulist = _admin.GetRoleMenuById(roleid);
			role.Name = rolename;
			role.Accounttype = (short)accounttype;
			_admin.UpdateRoles(role);
			_admin.SaveChanges();
			_admin.RemoveRangeRoleMenu(menulist);
		     _admin.SaveChanges();
			foreach (var item in rolemenu)
			{
				Rolemenu rolemenu1 = new Rolemenu();
				rolemenu1.Menuid = item;
				rolemenu1.Roleid = roleid;
				_admin.AddRoleMenus(rolemenu1);
			}
			_admin.SaveChanges(); 
			return RedirectToAction("Access", new { roleid = roleid });
		}
		#endregion

		#region DeleteRole
		[CustomAuthorize("Role", "7")]
		public IActionResult DeleteRole(int roleId)
		{
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

		#region UserAccessView
		public IActionResult UserAccess()
		{
			return View();
		}
		#endregion

		#region UserData
		public IActionResult UserData(int role)
		{
			var list = _admin.GetUserData(role);
			return PartialView("UserAccessPartial", list);
		}
		#endregion

		#region CreateAdmin
		public IActionResult CreateAdmin()
		{
			List<Role> roles = _context.Roles.Where(item => item.Accounttype == 1).ToList();
			List<Region> region = _admin.GetAllRegion();
			AdminProfileVm profile = new AdminProfileVm();
			profile.Regions = region;
			profile.Roles = roles;
			return View(profile);
		}
		#endregion

		[HttpPost]
		public IActionResult CreateAdmin(AdminProfileVm profileVm, int[] adminRegion)
		{
			AspnetUser? aspnetUser = _context.AspnetUsers.FirstOrDefault(item => item.Email == profileVm.Email);
			if (aspnetUser != null)
			{
				TempData["Error"] = "Send Request Unsuccessful!";
				return RedirectToAction("CreateAdmin", "Admin");
			}
			AspnetUser aspnet = new AspnetUser();
			aspnet.Email = profileVm.Email;
			aspnet.Aspnetuserid = Guid.NewGuid().ToString();
			aspnet.Username = profileVm.Username;
			aspnet.Createddat = DateTime.Now;
			aspnet.Passwordhash = BC.HashPassword(profileVm.Password);
			aspnet.Phonenumber = profileVm.MobileNo;
			_context.AspnetUsers.Add(aspnet);
			_context.SaveChanges();

			Admin admin = new Admin();
			admin.Email = profileVm.Email;
			admin.Aspnetuserid = aspnet.Aspnetuserid;
			admin.Firstname = profileVm.FirstName;
			admin.Lastname = profileVm.LastName;
			admin.Mobile = profileVm.MobileNo;
			admin.Address2 = profileVm.Address2;
			admin.Address1 = profileVm.Address1;
			admin.City = profileVm.City;
			admin.Regionid = profileVm.State;
			admin.Zip = profileVm.ZipCode;
			admin.Createdby = aspnet.Aspnetuserid;
			admin.Status = 1;

			_context.Admins.Add(admin);
			_context.SaveChanges();

			foreach (var region in adminRegion)
			{

				AdminRegion adminregions = new AdminRegion();
				adminregions.Adminid = admin.Adminid;
				adminregions.Regionid = region;
				_context.AdminRegions.Add(adminregions);
				_context.SaveChanges();
			}

			AspnetUserrole aspnetUserrole = new AspnetUserrole();
			aspnetUserrole.Userid = aspnet.Aspnetuserid;
			aspnetUserrole.Roleid = profileVm.RoleId;
			_context.AspnetUserroles.Add(aspnetUserrole);
			_context.SaveChanges();
			return RedirectToAction("UserAccess", "Admin");
		}
		//main View
		[CustomAuthorize("Dashboard", "6")]
		public IActionResult SendLink()
		{
			string? resetLink = Url.ActionLink("PatientRequest", "Request", protocol: HttpContext.Request.Scheme);
			if (_login.IsSendEmail("munavvarpopatiya999@gmail.com", "Munavvar", $"Click <a href='{resetLink}'>here</a> to Create A new Request"))
			{
				TempData["SuccessMessage"] = "Send Request successful!";
				return RedirectToAction("Index", "Admin");
			}
			else
			{
				TempData["Error"] = "Send Request Unsuccessful!";

				return RedirectToAction("Index", "Admin");
			}
		}
		#region Index
		[HttpGet("Admin/Dashboard", Name = "AdminDashboard")]
		public IActionResult Index()
		{
			List<NewRequestTableVM> request = _admin.GetAllData();
			int newcount = request.Count(item => item.Status == 1);
			return View(request.ToList());
		}
		#endregion

		#region SearchPatientDashboard
		[CustomAuthorize("Dashboard", "6")]
		public IActionResult SearchPatient(string searchValue, string selectValue, string partialName, string selectedFilter, int[] currentStatus, bool exportdata, bool exportAllData, int page, int pageSize = 5)
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
		#endregion

		#region ExportAll
		[CustomAuthorize("Dashboard", "6")]
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
		}
		#endregion

		#region PendingTablePartial
		public IActionResult PendingTablePartial()
		{
			return PartialView("PendingTablePartial");
		}
		#endregion

		#region ViewCaseGet
		[CustomAuthorize("Dashboard", "6")]
		public IActionResult ViewCase(int id)
		{
			ViewCaseVM ViewCase = _admin.GetCaseById(id);
			return View(ViewCase);
		}
		#endregion

		#region ViewCase
		[CustomAuthorize("Dashboard", "6")]
		[HttpPost]
		public async Task<IActionResult> ViewCase(ViewCaseVM viewCaseVM, int id)
		{
			if (ModelState.IsValid)
			{
				await _admin.UpdateRequestClient(viewCaseVM, id);
				return RedirectToAction("ViewCase", new { id });
			}
			return View();
		}
		#endregion

		#region ViewNotes
		[CustomAuthorize("Dashboard", "6")]
		public IActionResult Viewnotes(int requestid)
		{
			ViewData["ViewName"] = "Dashboard";
			ViewData["RequestId"] = requestid;
			List<ViewNotesVM> result = _admin.GetNotesForRequest(requestid);
			return View(result);
		}
		#endregion

		#region AssignRequest
		[CustomAuthorize("Dashboard", "6")]
		[HttpPost]
		public async Task<IActionResult> AssignRequest(int regionid, int physician, string description, int requestid, int status)
		{
			string? email = HttpContext.Session.GetString("Email");
			int? id = HttpContext.Session.GetInt32("id");
			Admin? admin = _admin.GetAdminByEmail(email);
			if (admin != null)
			{

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
		[CustomAuthorize("Dashboard", "6")]
		public async Task<IActionResult> TransferRequest(int regionid, int physician, string description, int requestid, int status)
		{
			string? email = HttpContext.Session.GetString("Email");
			int? id = HttpContext.Session.GetInt32("id");
			Admin? admin = _admin.GetAdminByEmail(email);
			if (admin != null)
			{

				bool result = await _admin.AssignRequest(regionid, physician, description, requestid, admin.Adminid);
				return Json(result);
			}
			else
			{
				return Json(false);
			}
		}
		#endregion

		#region ViewNotesPost
		[HttpPost]
		[CustomAuthorize("Dashboard", "6")]
		public async Task<IActionResult> ViewNotesPost(string adminNotes, int id)
		{

			if (adminNotes != null)
			{
				await _admin.UpdateAdminNotes(id, adminNotes);
			}
			return RedirectToAction("ViewNotes", new { requestid = id });
		}
		#endregion

		#region CancelCasePost
		[HttpPost]
		[CustomAuthorize("Dashboard", "6")]
		public async Task<IActionResult> CancelCase(string notes, string cancelReason, int requestid)
		{
			bool result = await _admin.CancelCase(requestid, notes, cancelReason);
			TempData["SuccessMessage"] = "Cancel successfully";
			return Json(result);
		}
		#endregion

		#region CreateRequest
		[CustomAuthorize("Dashboard", "6")]
		public IActionResult CreateRequest()
		{
			List<Region> region = _admin.GetAllRegion();
			RequestModel requestmoel = new RequestModel();
			requestmoel.Regions = region;
			return View(requestmoel);
		}
		#endregion

		
		[CustomAuthorize("Dashboard", "6")]
		[HttpPost]
		public IActionResult CreateRequest(RequestModel requestModel)
		{
			if (ModelState.IsValid)
			{
				string? email = HttpContext.Session.GetString("Email");
				int? id = HttpContext.Session.GetInt32("id");
				var statebyregionid = _admin.GetRegionByName(requestModel.State);

				Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
				if (admin != null)
				{
					_patient.AddRequest(requestModel, 0, 1);
					Request? request1 = _patient.GetRequestByEmail(requestModel.Email);
					if (request1 != null)
					{

						_patient.AddRequestClient(requestModel, request1.Requestid);


						_context.SaveChanges();

						Requestnote requestnote = new Requestnote();
						requestnote.Adminnotes = requestModel.Notes;
						requestnote.Createddate = DateTime.Now;
						requestnote.Requestid = request1.Requestid;
						requestnote.Createdby = "admin";
						_context.Requestnotes.Add(requestnote);
						_context.SaveChanges();
						int count = _context.Requests.Where(x => x.Createddate.Date == request1.Createddate.Date).Count() + 1;
						var region = _context.Regions.Where(x => x.Name == requestModel.State).FirstOrDefault();
						if (region != null)
						{
							var confirmNum = string.Concat(region?.Abbreviation?.ToUpper(), request1.Createddate.ToString("ddMMyy"), requestModel.Lastname.Substring(0, 2).ToUpper() ?? "",
						   requestModel.Firstname.Substring(0, 2).ToUpper(), count.ToString("D4"));
							request1.Confirmationnumber = confirmNum;
						}
						else
						{
							var confirmNum = string.Concat("ML", request1.Createddate.ToString("ddMMyy"), requestModel.Lastname.Substring(0, 2).ToUpper() ?? "",
						  requestModel.Firstname.Substring(0, 2).ToUpper(), count.ToString("D4"));
							request1.Confirmationnumber = confirmNum;
						}
						_context.Update(request1);
						_context.SaveChanges();
						string token = Guid.NewGuid().ToString();
						string? resetLink = Url.Action("Index", "Register", new { userId = request1.Requestid, token }, protocol: HttpContext.Request.Scheme);
						if (_login.IsSendEmail(requestModel.Email, "Munavvar", $"Click <a href='{resetLink}'>here</a> to Create A new Account"))
						{
							
							TempData["SuccessMessage"] = "Create Request successful!";
							return RedirectToAction("Index", "Admin");
						}
						else
						{

							return RedirectToAction("Index", "Admin");
						}
					}

				}
			}
			return View(requestModel);
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

		#region GetPhysician
		public IActionResult GetPhysician(string region)
		{
			try
			{
				var physicians = _admin.GetPhysiciansByRegion(region);
				return Ok(physicians);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}
		#endregion

		#region BlockRequest
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
		#endregion

		#region ViewUploads
		[CustomAuthorize("Dashboard", "6")]
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

		#region DeleteFile
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
		#endregion

		#region UploadFile
		[CustomAuthorize("Dashboard", "6")]
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
		#endregion

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

		#region GetOrder
		[CustomAuthorize("SendOrder", "12")]
		public IActionResult SendOrder(int requestid)
		{
			SendOrderModel sendOrderModel = _admin.GetSendOrder(requestid);

			return View(sendOrderModel);
		}
		#endregion

		#region SendOrder
		[CustomAuthorize("SendOrder", "12")]
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
		#endregion

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

		#region SendAgreement
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
		#endregion

		[HttpPost]
		public IActionResult ClearCase(int requestidclearcase)
		{
			try
			{
				string? email = HttpContext.Session.GetString("Email");
				int? id = HttpContext.Session.GetInt32("id");

				Admin? admin = _context.Admins.FirstOrDefault(item => item.Email == email);
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
		#region GeneratePDF
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
		#endregion
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
				return RedirectToAction("EncounterForm", "Admin", new { requestId = requestid });
			}
			else
			{
				// Handle finalization
				return RedirectToAction("Index", "Admin");
			}
		}

		public IActionResult CloseCase(int requestid)
		{

			Requestclient? requestclient = _admin.GetRequestClientById(requestid);
			List<Requestwisefile> requestwisedocument = _context.Requestwisefiles.Where(item => item.Requestid == requestid).ToList();
			Request? request = _admin.GetRequestById(requestid);
			if (request != null)
			{

				string? requestclientnumber = request.Confirmationnumber;
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
			else
			{
				TempData["Error"] = "Something Went Wrong Please Try Again";
				return RedirectToAction("Index", "Admin");
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
				Requestclient? requestclient = _admin.GetRequestClientById(requestid);
				if (requestclient != null)
				{

					requestclient.Phonenumber = closeCaseVM.PhoneNo;
					requestclient.Email = closeCaseVM.Email;
					_context.Requestclients.Update(requestclient);
				}
				_admin.SaveChanges();
				return RedirectToAction("CloseCase", new { requestid = requestid });
			}
		}

		#region CloseCaseModal
		public IActionResult CloseCaseModal(int requestid)
		{
			Request? request = _admin.GetRequestById(requestid);
			string? email = HttpContext.Session.GetString("Email");
			int? id = HttpContext.Session.GetInt32("id");
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
	}
}
