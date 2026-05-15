using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System.Linq;
using System.Text.Json;

namespace Web.Controllers
{
    public class PracticeController : Controller
    {
        private readonly EquationSolverService _solver;

        public PracticeController(EquationSolverService solver)
        {
            _solver = solver;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ЛИНЕЙНЫЕ УРАВНЕНИЯ (1 степень)

        [HttpGet]
        public IActionResult Linear()
        {
            ViewBag.Degree = 1;
            return View("PracticeLinear");
        }

        [HttpPost]
        public IActionResult Linear(int degree, bool integerRoots, bool realOnly, bool showSolution, string root1)
        {
            if (!showSolution)
            {
                TempData.Remove("CheckMessage");
                TempData.Remove("IsCorrect");
            }

            // Если это показ решения и есть переданные корни
            if (showSolution && !string.IsNullOrEmpty(root1))
            {
                var userRoots = new List<double>();
                if (!string.IsNullOrEmpty(root1)) userRoots.Add(ParseDouble(root1));
                ViewBag.LastRoots = userRoots;
            }

            if (showSolution && TempData.Peek("Equation") != null)
            {
                ViewBag.Equation = TempData.Peek("Equation")?.ToString();

                var rootsJson = TempData.Peek("Roots")?.ToString();
                if (!string.IsNullOrEmpty(rootsJson))
                {
                    ViewBag.Roots = JsonSerializer.Deserialize<List<EquationRoot>>(rootsJson);
                }

                var stepsJson = TempData.Peek("Steps")?.ToString();
                if (!string.IsNullOrEmpty(stepsJson))
                {
                    ViewBag.Steps = JsonSerializer.Deserialize<List<string>>(stepsJson);
                }

                ViewBag.Error = TempData.Peek("Error")?.ToString();
            }
            else
            {
                var result = _solver.SolveEquation(degree, integerRoots, realOnly, null);

                TempData["Equation"] = result.equation;
                TempData["Roots"] = JsonSerializer.Serialize(result.roots);
                TempData["Steps"] = JsonSerializer.Serialize(result.steps);
                TempData["Error"] = result.error;

                ViewBag.Equation = result.equation;
                ViewBag.Roots = result.roots;
                ViewBag.Steps = result.steps;
                ViewBag.Error = result.error;
            }

            ViewBag.Degree = degree;
            ViewBag.ShowSolution = showSolution;
            return View("PracticeLinear");
        }

        //КВАДРАТНЫЕ УРАВНЕНИЯ (2 степень)

        [HttpGet]
        public IActionResult Quadratic()
        {
            ViewBag.Degree = 2;
            return View("PracticeQuadratic");
        }

        [HttpPost]
        public IActionResult Quadratic(int degree, bool integerRoots, bool realOnly, bool showSolution,
            string root1, string root2)
        {
            if (!showSolution)
            {
                TempData.Remove("CheckMessage");
                TempData.Remove("IsCorrect");
            }

            // Если это показ решения и есть переданные корни
            if (showSolution && !string.IsNullOrEmpty(root1))
            {
                var userRoots = new List<double>();
                if (!string.IsNullOrEmpty(root1)) userRoots.Add(ParseDouble(root1));
                if (!string.IsNullOrEmpty(root2)) userRoots.Add(ParseDouble(root2));
                ViewBag.LastRoots = userRoots;
            }

            if (showSolution && TempData.Peek("Equation") != null)
            {
                ViewBag.Equation = TempData.Peek("Equation")?.ToString();

                var rootsJson = TempData.Peek("Roots")?.ToString();
                if (!string.IsNullOrEmpty(rootsJson))
                {
                    ViewBag.Roots = JsonSerializer.Deserialize<List<EquationRoot>>(rootsJson);
                }

                var stepsJson = TempData.Peek("Steps")?.ToString();
                if (!string.IsNullOrEmpty(stepsJson))
                {
                    ViewBag.Steps = JsonSerializer.Deserialize<List<string>>(stepsJson);
                }

                ViewBag.Error = TempData.Peek("Error")?.ToString();
            }
            else
            {
                var result = _solver.SolveEquation(degree, integerRoots, realOnly, null);

                TempData["Equation"] = result.equation;
                TempData["Roots"] = JsonSerializer.Serialize(result.roots);
                TempData["Steps"] = JsonSerializer.Serialize(result.steps);
                TempData["Error"] = result.error;

                ViewBag.Equation = result.equation;
                ViewBag.Roots = result.roots;
                ViewBag.Steps = result.steps;
                ViewBag.Error = result.error;
            }

            ViewBag.Degree = degree;
            ViewBag.ShowSolution = showSolution;
            return View("PracticeQuadratic");
        }

        //КУБИЧЕСКИЕ УРАВНЕНИЯ (3 степень)

        [HttpGet]
        public IActionResult Cubic()
        {
            ViewBag.Degree = 3;
            return View("PracticeCubic");
        }

        [HttpPost]
        public IActionResult Cubic(int degree, bool integerRoots, bool realOnly, bool showSolution,
            string root1, string root2, string root3)
        {
            if (!showSolution)
            {
                TempData.Remove("CheckMessage");
                TempData.Remove("IsCorrect");
            }

            // Если это показ решения и есть переданные корни
            if (showSolution && !string.IsNullOrEmpty(root1))
            {
                var userRoots = new List<double>();
                if (!string.IsNullOrEmpty(root1)) userRoots.Add(ParseDouble(root1));
                if (!string.IsNullOrEmpty(root2)) userRoots.Add(ParseDouble(root2));
                if (!string.IsNullOrEmpty(root3)) userRoots.Add(ParseDouble(root3));
                ViewBag.LastRoots = userRoots;
            }

            if (showSolution && TempData.Peek("Equation") != null)
            {
                ViewBag.Equation = TempData.Peek("Equation")?.ToString();

                var rootsJson = TempData.Peek("Roots")?.ToString();
                if (!string.IsNullOrEmpty(rootsJson))
                {
                    ViewBag.Roots = JsonSerializer.Deserialize<List<EquationRoot>>(rootsJson);
                }

                var stepsJson = TempData.Peek("Steps")?.ToString();
                if (!string.IsNullOrEmpty(stepsJson))
                {
                    ViewBag.Steps = JsonSerializer.Deserialize<List<string>>(stepsJson);
                }

                ViewBag.Error = TempData.Peek("Error")?.ToString();
            }
            else
            {
                var result = _solver.SolveEquation(degree, integerRoots, realOnly, null);

                TempData["Equation"] = result.equation;
                TempData["Roots"] = JsonSerializer.Serialize(result.roots);
                TempData["Steps"] = JsonSerializer.Serialize(result.steps);
                TempData["Error"] = result.error;

                ViewBag.Equation = result.equation;
                ViewBag.Roots = result.roots;
                ViewBag.Steps = result.steps;
                ViewBag.Error = result.error;
            }

            ViewBag.Degree = degree;
            ViewBag.ShowSolution = showSolution;
            return View("PracticeCubic");
        }

        //УРАВНЕНИЯ 4 СТЕПЕНИ

        [HttpGet]
        public IActionResult Quartic()
        {
            ViewBag.Degree = 4;
            return View("PracticeQuartic");
        }

        [HttpPost]
        public IActionResult Quartic(int degree, bool integerRoots, bool realOnly, bool showSolution,
            string root1, string root2, string root3, string root4)
        {
            if (!showSolution)
            {
                TempData.Remove("CheckMessage");
                TempData.Remove("IsCorrect");
            }

            // Если это показ решения и есть переданные корни
            if (showSolution && !string.IsNullOrEmpty(root1))
            {
                var userRoots = new List<double>();
                if (!string.IsNullOrEmpty(root1)) userRoots.Add(ParseDouble(root1));
                if (!string.IsNullOrEmpty(root2)) userRoots.Add(ParseDouble(root2));
                if (!string.IsNullOrEmpty(root3)) userRoots.Add(ParseDouble(root3));
                if (!string.IsNullOrEmpty(root4)) userRoots.Add(ParseDouble(root4));
                ViewBag.LastRoots = userRoots;
            }

            if (showSolution && TempData.Peek("Equation") != null)
            {
                ViewBag.Equation = TempData.Peek("Equation")?.ToString();

                var rootsJson = TempData.Peek("Roots")?.ToString();
                if (!string.IsNullOrEmpty(rootsJson))
                {
                    ViewBag.Roots = JsonSerializer.Deserialize<List<EquationRoot>>(rootsJson);
                }

                var stepsJson = TempData.Peek("Steps")?.ToString();
                if (!string.IsNullOrEmpty(stepsJson))
                {
                    ViewBag.Steps = JsonSerializer.Deserialize<List<string>>(stepsJson);
                }

                ViewBag.Error = TempData.Peek("Error")?.ToString();
            }
            else
            {
                var result = _solver.SolveEquation(degree, integerRoots, realOnly, null);

                TempData["Equation"] = result.equation;
                TempData["Roots"] = JsonSerializer.Serialize(result.roots);
                TempData["Steps"] = JsonSerializer.Serialize(result.steps);
                TempData["Error"] = result.error;

                ViewBag.Equation = result.equation;
                ViewBag.Roots = result.roots;
                ViewBag.Steps = result.steps;
                ViewBag.Error = result.error;
            }

            ViewBag.Degree = degree;
            ViewBag.ShowSolution = showSolution;
            return View("PracticeQuartic");
        }

        //ПРОВЕРКА КОРНЕЙ

        [HttpPost]
        public IActionResult CheckRoots(int degree, string root1, string root2, string root3, string root4)
        {
            var savedEquation = TempData.Peek("Equation")?.ToString();
            var savedRootsJson = TempData.Peek("Roots")?.ToString();
            var savedStepsJson = TempData.Peek("Steps")?.ToString();
            var savedError = TempData.Peek("Error")?.ToString();

            ViewBag.Equation = savedEquation;
            ViewBag.Steps = JsonSerializer.Deserialize<List<string>>(savedStepsJson ?? "[]");
            ViewBag.Error = savedError;

            var correctRootsList = JsonSerializer.Deserialize<List<EquationRoot>>(savedRootsJson ?? "[]");
            ViewBag.Roots = correctRootsList;

            var userRoots = new List<double>();
            bool parseError = false;

            try
            {
                if (degree == 1)
                {
                    userRoots.Add(ParseDouble(root1));
                }
                else if (degree == 2)
                {
                    userRoots.Add(ParseDouble(root1));
                    userRoots.Add(ParseDouble(root2));
                }
                else if (degree == 3)
                {
                    userRoots.Add(ParseDouble(root1));
                    userRoots.Add(ParseDouble(root2));
                    userRoots.Add(ParseDouble(root3));
                }
                else if (degree == 4)
                {
                    userRoots.Add(ParseDouble(root1));
                    userRoots.Add(ParseDouble(root2));
                    userRoots.Add(ParseDouble(root3));
                    userRoots.Add(ParseDouble(root4));
                }
            }
            catch
            {
                parseError = true;
                TempData["CheckMessage"] = "❌ Ошибка: неверный формат числа. Используйте десятичные дроби (например, 2.5 или -3.75)";
                TempData["IsCorrect"] = false;
                ViewBag.ShowSolution = false;
                return View(GetViewName(degree));
            }

            if (!parseError)
            {
                var correctRoots = correctRootsList.Select(r => r.Real).ToList();

                userRoots.Sort();
                correctRoots.Sort();

                bool isCorrect = true;
                for (int i = 0; i < userRoots.Count; i++)
                {
                    if (Math.Abs(userRoots[i] - correctRoots[i]) > 0.01)
                    {
                        isCorrect = false;
                        break;
                    }
                }

                if (isCorrect)
                {
                    TempData["CheckMessage"] = $"✅ Правильно! Корни найдены верно!<br/>" +
                                               $"<strong>Ваш ответ:</strong> {string.Join(", ", userRoots.Select(r => Math.Round(r, 2).ToString()))}<br/>" +
                                               $"<strong>Правильный ответ:</strong> {string.Join(", ", correctRoots.Select(r => Math.Round(r, 2).ToString()))}";
                    TempData["IsCorrect"] = true;
                    ViewBag.ShowSolution = false;
                }
                else
                {
                    TempData["CheckMessage"] = $"❌ Неправильно.<br/>" +
                                               $"<strong>Ваш ответ:</strong> {string.Join(", ", userRoots.Select(r => Math.Round(r, 2).ToString()))}<br/>" +
                                               $"Нажмите 'Показать решение'.";
                    TempData["IsCorrect"] = false;
                    ViewBag.ShowSolution = false;
                }
            }
            ViewBag.LastRoots = userRoots;

            return View(GetViewName(degree));
        }

        //ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ

        private double ParseDouble(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Пустое значение");

            input = input.Trim().Replace(',', '.');

            if (!double.TryParse(input, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double result))
            {
                throw new ArgumentException("Неверный формат числа");
            }

            return result;
        }

        private string GetViewName(int degree)
        {
            return degree switch
            {
                1 => "PracticeLinear",
                2 => "PracticeQuadratic",
                3 => "PracticeCubic",
                4 => "PracticeQuartic",
                _ => "PracticeLinear"
            };
        }
    }
}