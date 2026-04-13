using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class PracticeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Linear()
        {
            return View();
        }

        public IActionResult Quadratic()
        {
            return View();
        }

        public IActionResult Cubic()
        {
            return View();
        }

        public IActionResult Quartic()
        {
            return View();
        }
    }
}
