using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System.Text.Json;  // Добавляем для сериализации (преобразования объектов в JSON-строку и обратно)

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

        // ========== ЛИНЕЙНЫЕ УРАВНЕНИЯ (1 степень) ==========

        [HttpGet]
        public IActionResult Linear()
        {
            ViewBag.Degree = 1;
            return View("PracticeLinear");
        }

        [HttpPost]
        public IActionResult Linear(int degree, bool integerRoots, bool realOnly, bool showSolution)
        {
            // TempData - это хранилище, которое сохраняет данные между HTTP-запросами
            // Обычно TempData живёт до тех пор, пока данные не будут прочитаны (один раз)
            // Мы используем TempData, чтобы сохранить сгенерированное уравнение, корни и шаги
            // между нажатиями кнопок "Сгенерировать", "Показать решение" и "Новое уравнение"

            // showSolution = false: пользователь нажал "Сгенерировать" или "Новое уравнение"
            // showSolution = true: пользователь нажал "Показать решение"

            if (showSolution && TempData["Equation"] != null)
            {
                // Случай 1: Показываем решение ранее сгенерированного уравнения
                // Достаём сохранённые данные из TempData
                ViewBag.Equation = TempData["Equation"]?.ToString();

                // Десериализуем JSON обратно в список корней
                var rootsJson = TempData["Roots"]?.ToString();
                if (!string.IsNullOrEmpty(rootsJson))
                {
                    ViewBag.Roots = JsonSerializer.Deserialize<List<EquationRoot>>(rootsJson);
                }

                // Десериализуем JSON обратно в список шагов
                var stepsJson = TempData["Steps"]?.ToString();
                if (!string.IsNullOrEmpty(stepsJson))
                {
                    ViewBag.Steps = JsonSerializer.Deserialize<List<string>>(stepsJson);
                }

                ViewBag.Error = TempData["Error"]?.ToString();
            }
            else
            {
                // Случай 2: Генерируем НОВОЕ уравнение
                var result = _solver.SolveEquation(degree, integerRoots, realOnly, null);

                // Сохраняем результат в TempData для будущих запросов "Показать решение"
                TempData["Equation"] = result.equation;
                // Сериализуем список корней в JSON (чтобы сохранить в TempData)
                TempData["Roots"] = JsonSerializer.Serialize(result.roots);
                // Сериализуем список шагов в JSON
                TempData["Steps"] = JsonSerializer.Serialize(result.steps);
                TempData["Error"] = result.error;

                // Также передаём в ViewBag для отображения на текущей странице
                ViewBag.Equation = result.equation;
                ViewBag.Roots = result.roots;
                ViewBag.Steps = result.steps;
                ViewBag.Error = result.error;
            }

            ViewBag.Degree = degree;
            ViewBag.ShowSolution = showSolution;
            return View("PracticeLinear");
        }

        // ========== КВАДРАТНЫЕ УРАВНЕНИЯ (2 степень) ==========

        [HttpGet]
        public IActionResult Quadratic()
        {
            ViewBag.Degree = 2;
            return View("PracticeQuadratic");
        }

        [HttpPost]
        public IActionResult Quadratic(int degree, bool integerRoots, bool realOnly, bool showSolution)
        {
            // Аналогичная логика, что и для линейных уравнений
            // TempData сохраняет данные между запросами

            if (showSolution && TempData["Equation"] != null)
            {
                // Показываем сохранённое решение
                ViewBag.Equation = TempData["Equation"]?.ToString();

                var rootsJson = TempData["Roots"]?.ToString();
                if (!string.IsNullOrEmpty(rootsJson))
                {
                    ViewBag.Roots = JsonSerializer.Deserialize<List<EquationRoot>>(rootsJson);
                }

                var stepsJson = TempData["Steps"]?.ToString();
                if (!string.IsNullOrEmpty(stepsJson))
                {
                    ViewBag.Steps = JsonSerializer.Deserialize<List<string>>(stepsJson);
                }

                ViewBag.Error = TempData["Error"]?.ToString();
            }
            else
            {
                // Генерируем новое уравнение и сохраняем в TempData
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

        // ========== КУБИЧЕСКИЕ УРАВНЕНИЯ (3 степень) ==========

        [HttpGet]
        public IActionResult Cubic()
        {
            ViewBag.Degree = 3;
            return View("PracticeCubic");
        }

        [HttpPost]
        public IActionResult Cubic(int degree, bool integerRoots, bool realOnly, bool showSolution)
        {
            // Аналогичная логика
            // TempData работает как временное хранилище между разными HTTP-запросами
            // Например: между запросом "Сгенерировать" и запросом "Показать решение"

            if (showSolution && TempData["Equation"] != null)
            {
                // Показываем ранее сохранённое решение (без генерации нового уравнения)
                ViewBag.Equation = TempData["Equation"]?.ToString();

                var rootsJson = TempData["Roots"]?.ToString();
                if (!string.IsNullOrEmpty(rootsJson))
                {
                    ViewBag.Roots = JsonSerializer.Deserialize<List<EquationRoot>>(rootsJson);
                }

                var stepsJson = TempData["Steps"]?.ToString();
                if (!string.IsNullOrEmpty(stepsJson))
                {
                    ViewBag.Steps = JsonSerializer.Deserialize<List<string>>(stepsJson);
                }

                ViewBag.Error = TempData["Error"]?.ToString();
            }
            else
            {
                // Генерируем новое уравнение
                var result = _solver.SolveEquation(degree, integerRoots, realOnly, null);

                // Сохраняем в TempData для будущего показа решения
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

        // ========== УРАВНЕНИЯ 4 СТЕПЕНИ ==========

        [HttpGet]
        public IActionResult Quartic()
        {
            ViewBag.Degree = 4;
            return View("PracticeQuartic");
        }

        [HttpPost]
        public IActionResult Quartic(int degree, bool integerRoots, bool realOnly, bool showSolution)
        {
            // Аналогичная логика для уравнений 4 степени

            if (showSolution && TempData["Equation"] != null)
            {
                // Показываем сохранённое решение
                ViewBag.Equation = TempData["Equation"]?.ToString();

                var rootsJson = TempData["Roots"]?.ToString();
                if (!string.IsNullOrEmpty(rootsJson))
                {
                    ViewBag.Roots = JsonSerializer.Deserialize<List<EquationRoot>>(rootsJson);
                }

                var stepsJson = TempData["Steps"]?.ToString();
                if (!string.IsNullOrEmpty(stepsJson))
                {
                    ViewBag.Steps = JsonSerializer.Deserialize<List<string>>(stepsJson);
                }

                ViewBag.Error = TempData["Error"]?.ToString();
            }
            else
            {
                // Генерируем новое уравнение
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
    }
}