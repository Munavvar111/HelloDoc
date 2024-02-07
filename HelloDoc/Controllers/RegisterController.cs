using Microsoft.AspNetCore.Mvc;

namespace HalloDocPatient.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormCollection collection)
        {
            if(collection.Count < 5)
            {
                return RedirectToAction("Index","Dashboard");
            }
            return Json(collection);
           
        }

    }
}
