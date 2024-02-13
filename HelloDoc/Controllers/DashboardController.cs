using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HalloDocPatient.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _patientRequest;

        public DashboardController(ApplicationDbContext context, IPatientRequest patientRequest)
        {
            _context = context;
            _patientRequest = patientRequest;
        }
        public IActionResult ViewDocument()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            var email = HttpContext.Session.GetString("Email");

            var result = from req in _context.Requests
                         join reqFile in _context.Requestwisefiles
                         on req.Requestid equals reqFile.Requestid into reqFilesGroup
                         where req.Email == email
                         from reqFile in reqFilesGroup.DefaultIfEmpty()
                         select new PatientDashboard
                         {
                             CreatedDate = reqFile != null ? reqFile.Createddate : req.Createddate,
                             Status = req.Status,
                             Filename = reqFile != null ? reqFile.Filename : null
                         };

            ViewBag.Email = email;

            return View(result.ToList());
        }


    }

}
