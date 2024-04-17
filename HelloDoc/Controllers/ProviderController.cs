using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HelloDoc.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProvider _provider;
        private readonly IAdmin _admin;
        private readonly ILogin _login;
        public ProviderController(IProvider provider, IAdmin admin,ILogin login)
        {
            _provider = provider;
            _login= login;  
            _admin = admin;
        }
        [CustomAuthorize("Dashboard", "19")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FilterPatient(string searchValue, string partialName, string selectedFilter, int[] currentStatus, int page, int pageSize = 5)
        {
            if (page == 0)
            {
                page = 1;
            }
            string? Email = HttpContext.Session.GetString("Email");
            List<NewRequestTableVM> filteredPatients = _provider.SearchPatients(searchValue, selectedFilter, currentStatus, Email);
            int totalItems = filteredPatients.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            List<NewRequestTableVM> paginatedData = filteredPatients.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.totalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.IsPhysician = true;

            return PartialView(partialName, paginatedData);
        }
        public IActionResult ViewCase(int id)
        {
            return RedirectToAction("ViewCase", "Admin", new { id = id });
        }
        public IActionResult ViewNotes(int requestid)
        {
            return RedirectToAction("Viewnotes", "Admin", new { id = requestid });
        }
        public IActionResult Accept(int id)
        {
            string? Email = HttpContext.Session.GetString("Email");
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            bool accepted = _provider.RequestAcceptedByProvider(id,physician.Physicianid);
            if (accepted)
            {
                TempData["SuccessMessage"] = "Accepted successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Accepted Unsuccessfully";
                return RedirectToAction("Index");
            }
            
        }
        public IActionResult HouseCall(int requestidHouse)
        {
            Request request=_admin.GetRequestById(requestidHouse);
            request.Calltype = 1;
            _admin.UpdateRequest(request);
            _admin.SaveChanges();
            TempData["SuccessMessage"] = "House call has been successfully processed.";

            return RedirectToAction("Index");

        }
        public IActionResult ConfirmHouseCall(int id)
        {
            Request request= _admin.GetRequestById(id);
            request.Status = 6;
            _admin.UpdateRequest(request);
                _admin.SaveChanges();
            return Ok();

        }
		public bool UpdatePhysicianLocation(decimal latitude, decimal longitude)
		{
			int? physicianId = HttpContext.Session.GetInt32("PhysicianId");
			if (physicianId == null)
			{
				return false;
			}
			else
			{
				return _admin.UpdatePhysicianLocation(latitude, longitude, (int)physicianId);
			}
		}

		public IActionResult Consult(int requestidConsult)
        {
            Request request = _admin.GetRequestById(requestidConsult);
            request.Status = 6;
            _admin.UpdateRequest(request);
            _admin.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult ViewUploads(int id)
        {
            return RedirectToAction("ViewUploads", "Admin", new { id = id });

        }
        public IActionResult EncounterForm(int requestid)
        {
            return RedirectToAction("EncounterForm", "Admin", new { requestId = requestid });

        }

        public IActionResult GeneratePDF(int requestid)
        {
            return RedirectToAction("GeneratePDF", "Admin", new { requestid = requestid }); 
        }
        public IActionResult Scheduling()
        {
            return RedirectToAction("Scheduling", "Admin");
        }
        public IActionResult CancelHouseCall(int id)
        {
            Request request=_admin.GetRequestById(id);
            request.Calltype = null; _admin.UpdateRequest(request);
            _admin.SaveChanges();
            
            return Ok();

        }

        public IActionResult ConcludeCare(int requestid)
        {
            return RedirectToAction("CloseCase", "Admin", new { requestid = requestid });
        }
        public IActionResult CreateRequest()
        {
            return RedirectToAction("CreateRequest", "Admin");
        }
        

        public IActionResult RequestToAdminForEdit(int requestid,string editadminnotes)
        {
			string? requestlink = Url.ActionLink("GrantAccessOfEdit", "Admin", new {id=requestid} ,protocol: HttpContext.Request.Scheme);
            Physician physician = _admin.GetPhysicianById(requestid);
            string physicianname = physician.Firstname;
			if (_login.IsSendEmail("munavvarpopatiya999@gmail.com", "Munavvar", $"Click <a href='{requestlink}'>here</a> to Grant Access {physicianname} Of Edit Physician Account "))
            {
				TempData["SuccessMessage"] = "Email Send ScueessFully.";
                return RedirectToAction("PhysicanProfile", "Provider",new{id=requestid});
			}
            else
            {
				TempData["Error"] = "Email Unsend.";
				return RedirectToAction("PhysicanProfile", "Provider");
            }


		}
        public bool TransferCaseProvider(string TransferDec,int requestid)
        {
            Request? request =_admin.GetRequestById(requestid);
            string? Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                return false;
            }
            Physician? physician = _admin.GetPhysicianByEmail(Email);
            if (physician != null && request != null)
            {
                try
                {
                    Requeststatuslog requeststatuslog = new Requeststatuslog
                    {
                        Requestid = requestid,
                        Notes = TransferDec,
                        Status = 2,
                        Physicianid = physician.Physicianid,
                        Transtoadmin = new System.Collections.BitArray(new[] {true}),
                        Createddate = DateTime.Now,
                    };
                    _admin.UpdateRequestStatusLog(requeststatuslog);
                    _admin.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to submit Form", ex);
                }
            }
            else
            {
                return false;
            }
        }

    }
}
