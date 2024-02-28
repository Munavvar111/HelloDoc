using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
            ViewBag.CurrentPage = page;
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
        public async Task<IActionResult> ViewNotes(int requestid)
        {
            var notes = (from reqnotes in _context.Requestnotes
                         join reqstatuslog in _context.Requeststatuslogs
                         on reqnotes.Requestid equals reqstatuslog.Requestid into requestAllNotes
                         where reqnotes.Requestid == requestid
                         group requestAllNotes by new
                         {
                             reqnotes.Adminnotes,
                             reqnotes.Physiciannotes,
                             reqnotes.Requestid
                         } into grouped
                         select new ViewNotesVM
                         {
                             TransferedNotes = grouped.SelectMany(g => g.Select(note => note.Notes)).DefaultIfEmpty().ToArray(),
                             AdminNotes = grouped.Key.Adminnotes,
                             PhysicianNotes = grouped.Key.Physiciannotes,
                             RequestId = grouped.Key.Requestid
                         }).FirstOrDefault();

            return View(notes);
        }


        [HttpPost]
        public async Task<IActionResult> ViewNotesPost(ViewNotesVM viewNotesVM)
        {
            if (ModelState.IsValid)
            {
                var requestvisenotes = _context.Requestnotes.Where(item => item.Requestid == viewNotesVM.RequestId).FirstOrDefault();
                if (requestvisenotes != null) {
                    
                    requestvisenotes.Adminnotes = viewNotesVM.AdditionalNotes;
                    _context.Update(requestvisenotes);
                     _context.SaveChanges();
                }
                else
                {
                    var requestnotes = new Requestnote();
                    requestnotes.Requestid= viewNotesVM.RequestId;
                    requestnotes.Adminnotes= viewNotesVM.AdditionalNotes;
                    requestnotes.Createddate = DateTime.Now;
                    requestnotes.Createdby = "admin";

                    _context.Add(requestnotes);
                    _context.SaveChanges(); 
                }
            }
            return RedirectToAction("ViewNotes",new { requestid =viewNotesVM.RequestId});
        }

        public async Task<IActionResult> CancelCase(string notes,string CancelReason,int Id)
        {
            var requestByid=await _context.Requests.FindAsync(Id); 
            if (requestByid != null)
            {
                requestByid.Status = 3;
                _context.Requests.Update(requestByid);
                
                var requeststatuslog=new Requeststatuslog();    
                requeststatuslog.Status = 5;
                requeststatuslog.Requestid = Id;
                requeststatuslog.Notes=notes;
                requeststatuslog.Createddate = DateTime.Now;

                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.SaveChanges();
                return Json(new {success=true});
            }
            return Json(new {success=false,message="Requestis not found"});

        }
    }
}
