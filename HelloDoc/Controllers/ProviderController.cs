using BusinessLayer.InterFace;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HelloDoc.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProvider _provider;
        private readonly IAdmin _admin;
        public ProviderController(IProvider provider, IAdmin admin)
        {
            _provider = provider;
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
           bool accepted = _provider.RequestAcceptedByProvider(id);
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
    }
}
