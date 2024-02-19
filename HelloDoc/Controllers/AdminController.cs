using BusinessLayer.Repository;
using DataAccessLayer.DataContext;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var request = from req in _context.Requests
                          join reqclient in _context.Requestclients
                          on req.Requestid equals reqclient.Requestid
                          select req ;
            return View(request);
        }
    }
}
