using Microsoft.AspNetCore.Mvc;

namespace HalloDocPatient.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
