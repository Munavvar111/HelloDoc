using Microsoft.AspNetCore.Mvc;

namespace HalloDocPatient.Controllers
{
    public class RequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BusinessRequest()
        {
            return View();
        }
        public IActionResult PatientRequest()
        {
            return View();
        }
        public IActionResult FriendRequest()
        {
            return View();
        }
        public IActionResult ConceirgeRequest()
        {
            return View();
        }


    }
}
