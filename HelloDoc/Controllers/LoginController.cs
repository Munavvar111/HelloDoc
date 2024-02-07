using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocPatient.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AspnetUser a)
        
        {
            if(ModelState.IsValid)
            {
                Console.WriteLine("hii");
            var username = a.Username;
            var password = a.Passwordhash;

            var user = await _context.AspnetUsers.FirstOrDefaultAsync(m => (m.Username == username) && (m.Passwordhash == password));
            if (user != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            }
            return View(a);
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitFORM(IFormCollection collection)
        {
            return Json(collection);
        }



    }
}
