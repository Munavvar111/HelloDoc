using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;

namespace HelloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdmin _admin;
        public AdminController(ApplicationDbContext context,IAdmin admin)
        {
            _context = context;
            _admin=admin;
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

        
        public IActionResult SearchPatient(string searchValue,string selectValue,string partialName,string selectedFilter, int[] currentStatus,int page=1,int pageSize=3)
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
        public async Task<IActionResult> AssignRequest( int regionid, int physician, string description, int requestid)
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
    }
}
