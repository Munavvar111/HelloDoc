using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System.Linq;

namespace HelloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ProviderLocation()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Provider()
        {
            return View();  
        }

        public IActionResult Parteners()
        {
            return View();
        }
        public IActionResult Records()
        {
            return View();
        }

        public IActionResult Index()
        {
            
            var request = from req in _context.Requests
                          join reqclient in _context.Requestclients
                          on req.Requestid equals reqclient.Requestid
                          select new NewRequestTableVM
                          {
                              PatientName = reqclient.Firstname,
                              Requestor = req.Firstname + " " + req.Lastname,
                              DateOfBirth = new DateTime((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                              ReqDate = req.Createddate,
                              Phone = req.Phonenumber,
                              Address = reqclient.City + reqclient.Zipcode,
                              Notes = reqclient.Notes,
                              ReqTypeId=req.Requesttypeid,
                              Email=req.Email,
                              Id=reqclient.Requestclientid
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
                              DateOfBirth = new DateTime((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                              ReqDate = req.Createddate,
                              Phone = req.Phonenumber,
                              Address = reqclient.City + reqclient.Zipcode,
                              Notes = reqclient.Notes
,
                          };
            return Json(new {data=request.ToList()}, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public IActionResult SearchPatient(string searchValue)
        {
            // Assuming _context is your DbContext
            var filteredPatients = (from req in _context.Requests
                                   join reqclient in _context.Requestclients
                                   on req.Requestid equals reqclient.Requestid
                                   select new NewRequestTableVM
                                   {
                                       PatientName = reqclient.Firstname,
                                       Requestor = req.Firstname + " " + req.Lastname,
                                       DateOfBirth = new DateTime((int)reqclient.Intyear, int.Parse(reqclient.Strmonth), (int)reqclient.Intdate),
                                       ReqDate = req.Createddate,
                                       Phone = req.Phonenumber,
                                       Address = reqclient.City + reqclient.Zipcode,
                                       Notes = reqclient.Notes
         ,
                                   }).Where(patient=>patient.PatientName.Contains(searchValue)).ToList();

            return PartialView("NewTablePartial", filteredPatients);
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
    }
}
