using DataAccessLayer.CustomModel;
using DataAccessLayer.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocPatient.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            avar PatientDetails = (from req in _context.Requests
                                  join refi in _context.Requestwisefiles on req.Requestid equals refi.Requestid into requestFiles
                                  from file in requestFiles.DefaultIfEmpty()
                                  select new PatientDashboard
                                  {
                                      CreatedDate = req.Createddate,
                                      Status = req.Status,
                                      Filename = file != null ? file.Filename : null
                                  }).ToList();

            return View(PatientDetails);
        }

    }

}
