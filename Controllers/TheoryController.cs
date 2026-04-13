using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class TheoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Linear() // Линейные уравнения 
        {
            return View();
        }

        public IActionResult Quadratic() // Квадратные 
        {
            return View();
        }

        public IActionResult Cubic() // Кубические 
        {
            return View();
        }

        public IActionResult Quartic() // 4 степень 
        {
            return View();
        }
    }
}

