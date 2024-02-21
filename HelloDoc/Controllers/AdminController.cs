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
                              Phone = req.Phonenumber,
                              Address = reqclient.City + reqclient.Zipcode,
                              Notes = reqclient.Notes,
                              ReqTypeId=req.Requesttypeid,
                              Email=req.Email,
                              Id=reqclient.Requestclientid,
                              Status=req.Status
,
                          };


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
                              Status = req.Status

,
                          };
            return Json(new {data=request.ToList()}, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }
        public IActionResult SearchPatient(string searchValue,string selectValue,string partialName,string selectedFilter)
        {
            var filteredPatients = _admin.SearchPatients(searchValue, selectValue, selectedFilter);
            return PartialView(partialName, filteredPatients);
        }

        public IActionResult Learning()
        {
            return View();
        }


        public IActionResult AjaxMethod()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;

                // getting all Customer data  
                var customerData = (from req in _context.Requests
                                    join reqfile in _context.Requestclients
                                    on req.Requestid equals reqfile.Requestid
                                    select new
                                    {
                                        requestid=req.Requestid,
                                        firstname=req.Firstname,
                                        lastname=req.Lastname,
                                        email=req.Email
                                    }).ToList();
                //Sorting  

                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(item => item.firstname.Contains(searchValue)).ToList();
                }


                //total number of rows counts   
                recordsTotal = customerData.Count();
                //Paging   
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
               return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }
        public IActionResult NewPartial(string partialName)
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
                              Status=req.Status
,
                          };
            return PartialView(partialName, request);
        }

        public IActionResult PendingTablePartial()
        {
            return PartialView("PendingTablePartial");
        }
    }
}
