using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System.Drawing;
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

            var request = from req in _context.Requests
                          join reqclient in _context.Requestclients
                          on req.Requestid equals reqclient.Requestid
                          select new NewRequestTableVM
                          {
                              PatientName = reqclient.Firstname,
                              Requestor = req.Firstname + " " + req.Lastname,
                              DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                              ReqDate = req.Createddate,
                              Phone = reqclient.Phonenumber,
                              Address = reqclient.City + reqclient.Zipcode,
                              Notes = reqclient.Notes,
                              ReqTypeId=req.Requesttypeid,
                              Email=req.Email,
                              Id=reqclient.Requestclientid,
                              Status=req.Status,
                              PhoneOther=req.Phonenumber,
                              RequestClientId=reqclient.Requestclientid,
                          };

            

            var newcount = (_context.Requests.Where(item => item.Status == 1)).Count();
            var pandingcount=(_context.Requests.Where(item => item.Status==2)).Count();
            var activecount=(_context.Requests.Where(item => item.Status==4 || item.Status==5)).Count();
            var conclude=(_context.Requests.Where(item => item.Status==6)).Count();
            var toclosed=(_context.Requests.Where(item => item.Status==3 || item.Status==7 || item.Status==8)).Count();
            var unpaid=(_context.Requests.Where(item => item.Status==9)).Count();
            ViewBag.PandingCount = pandingcount;
            ViewBag.NewCount=newcount;
            ViewBag.activecount = activecount;
            ViewBag.conclude = conclude;
            ViewBag.toclosed = toclosed;
            ViewBag.unpaid = unpaid;
            return View(request.ToList());
        }   

        public IActionResult GetData()
        {
            var request = from req in _context.Requests
                          join reqclient in _context.Requestclients
                          on req.Requestid equals reqclient.Requestid
                          select new NewRequestTableVM
                          {
                              PatientName = reqclient.Firstname,
                              Requestor = req.Firstname + " " + req.Lastname,
                              DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                              ReqDate = req.Createddate,
                              Phone = req.Phonenumber,
                              Address = reqclient.City + reqclient.Zipcode,
                              Notes = reqclient.Notes,
                              ReqTypeId = req.Requesttypeid,
                              Email = req.Email,
                              Id = reqclient.Requestclientid,
                              Status = req.Status,
                          };
            return Json(new {data=request.ToList()}, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }
        public IActionResult SearchPatient(string searchValue,string selectValue,string partialName,string selectedFilter, int[] currentStatus)
        {
            var filteredPatients = _admin.SearchPatients(searchValue, selectValue, selectedFilter, currentStatus);
                return PartialView(partialName, filteredPatients);
        }

        public IActionResult Learning()
        {
            return View();
        }


       
        public IActionResult NewPartial(string partialName,int[] currentStatus,int page , int pageSize = 3)
            {
            var request = (from req in _context.Requests
                          join reqclient in _context.Requestclients
                          on req.Requestid equals reqclient.Requestid
                          select new NewRequestTableVM
                          {
                              PatientName = reqclient.Firstname,
                              Requestor = req.Firstname + " " + req.Lastname,
                              DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                              ReqDate = req.Createddate,
                              Phone = reqclient.Phonenumber,
                              Address = reqclient.City + reqclient.Zipcode,
                              Notes = reqclient.Notes,
                              ReqTypeId = req.Requesttypeid,
                              Email = req.Email,
                              Id = reqclient.Requestclientid,
                              Status=req.Status,
                              PhoneOther=req.Phonenumber,
                              RequestClientId= reqclient.Requestclientid,
                          }).Where(item => currentStatus.Any(status=>item.Status==status));

            int totalItems = request.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedRequest = request.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return PartialView(partialName, paginatedRequest);
        }

        public IActionResult PendingTablePartial()
        {
            return PartialView("PendingTablePartial");
        }
        public IActionResult ViewCase(int id)
        {
            var requestclient = (from req in _context.Requests
                                join reqclient in _context.Requestclients
                                on req.Requestid equals reqclient.Requestid
                                where reqclient.Requestclientid == id
                                select new ViewCaseVM
                                {
                                    Notes=reqclient.Notes,
                                    FirstName=reqclient.Firstname,
                                    LastName=reqclient.Lastname,
                                    DateOfBirth = new DateOnly((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                                    Phone=reqclient.Phonenumber,
                                    Email=reqclient.Email,
                                    Location=reqclient.Location,
                                    RequestClientId=reqclient.Requestclientid,
                                }).FirstOrDefault();
                               
            return View(requestclient);
        }

        [HttpPost]
        public async Task<IActionResult> ViewCase(ViewCaseVM viewCaseVM,int id)
        {
            var requestclient = await _context.Requestclients.FindAsync(id);
            if (ModelState.IsValid)
            {
                requestclient.Firstname = viewCaseVM.FirstName;
                requestclient.Lastname = viewCaseVM.LastName;
                requestclient.Location = viewCaseVM.Location;
                requestclient.Email= viewCaseVM.Email;
                requestclient.Notes= viewCaseVM.Notes;
                requestclient.Phonenumber = viewCaseVM.Phone;
                requestclient.Intdate = viewCaseVM.DateOfBirth.Day; 
                requestclient.Intyear = viewCaseVM.DateOfBirth.Year;
                requestclient.Strmonth = viewCaseVM.DateOfBirth.Month.ToString();

                _context.Update(requestclient);
                _context.SaveChanges();

                return RedirectToAction("ViewCase", new { id });
            }
            return View();  
        }
    }
}
