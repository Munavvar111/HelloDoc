using Microsoft.AspNetCore.Mvc;

namespace HelloDoc.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
