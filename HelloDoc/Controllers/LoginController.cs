using Microsoft.AspNetCore.Mvc;

namespace HalloDocPatient.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
