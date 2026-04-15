using Microsoft.AspNetCore.Mvc;
using Web.Services;

namespace Web.Controllers
{
    public class EquationController : Controller
    {
        private readonly EquationSolverService _solver;

       
        public EquationController(EquationSolverService solver)
        {
            _solver = solver;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Solve(int degree, bool integerRoots, bool realOnly, string equationText)
        {
            var result = _solver.SolveEquation(degree, integerRoots, realOnly, equationText);
            return Json(result);
        }
    }
}