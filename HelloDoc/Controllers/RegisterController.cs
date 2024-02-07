using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocPatient.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult View1()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AspnetUser ac)
        {     
            
                AspnetUser user = new AspnetUser();
                user.Aspnetuserid = ac.Username;
               user.Username = ac.Username;
                user.Passwordhash = ac.Passwordhash;
                _context.AspnetUsers.Add(user);
                 _context.SaveChanges();
                return RedirectToAction(nameof(View1));
            
            
        }

    }
}
