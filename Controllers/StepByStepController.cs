using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Web.Controllers
{
    public class StepByStepController : Controller
    {
        // ГЛАВНАЯ СТРАНИЦА ВЫБОРА СТЕПЕНИ 
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        private string FormatEquation(double a, double b, double c = 0, double d = 0, double e = 0, int degree = 1)
        {
            var parts = new List<string>();

            if (degree == 1)
            {
                // ax + b = 0
                if (a != 0)
                {
                    string aStr = (Math.Abs(a) == 1) ? (a == 1 ? "" : "-") : FormatNumber(a);
                    parts.Add($"{aStr}x");
                }
                if (b != 0)
                {
                    string sign = b > 0 ? " + " : " - ";
                    string bStr = FormatNumber(Math.Abs(b));
                    parts.Add($"{sign}{bStr}");
                }
                else if (a == 0 && b == 0)
                {
                    parts.Add("0");
                }
                parts.Add(" = 0");
                return string.Join("", parts);
            }
            else if (degree == 2)
            {
                // ax² + bx + c = 0
                if (a != 0)
                {
                    string aStr = (Math.Abs(a) == 1) ? (a == 1 ? "" : "-") : FormatNumber(a);
                    parts.Add($"{aStr}x²");
                }
                if (b != 0)
                {
                    string sign = b > 0 ? " + " : " - ";
                    string bStr = (Math.Abs(b) == 1) ? "" : FormatNumber(Math.Abs(b));
                    parts.Add($"{sign}{bStr}x");
                }
                if (c != 0)
                {
                    string sign = c > 0 ? " + " : " - ";
                    string cStr = FormatNumber(Math.Abs(c));
                    parts.Add($"{sign}{cStr}");
                }
                parts.Add(" = 0");
                return string.Join("", parts);
            }
            else if (degree == 3)
            {
                // ax³ + bx² + cx + d = 0
                if (a != 0)
                {
                    string aStr = (Math.Abs(a) == 1) ? (a == 1 ? "" : "-") : FormatNumber(a);
                    parts.Add($"{aStr}x³");
                }
                if (b != 0)
                {
                    string sign = b > 0 ? " + " : " - ";
                    string bStr = (Math.Abs(b) == 1) ? "" : FormatNumber(Math.Abs(b));
                    parts.Add($"{sign}{bStr}x²");
                }
                if (c != 0)
                {
                    string sign = c > 0 ? " + " : " - ";
                    string cStr = (Math.Abs(c) == 1) ? "" : FormatNumber(Math.Abs(c));
                    parts.Add($"{sign}{cStr}x");
                }
                if (d != 0)
                {
                    string sign = d > 0 ? " + " : " - ";
                    string dStr = FormatNumber(Math.Abs(d));
                    parts.Add($"{sign}{dStr}");
                }
                parts.Add(" = 0");
                return string.Join("", parts);
            }
            else if (degree == 4)
            {
                // ax⁴ + bx³ + cx² + dx + e = 0
                if (a != 0)
                {
                    string aStr = (Math.Abs(a) == 1) ? (a == 1 ? "" : "-") : FormatNumber(a);
                    parts.Add($"{aStr}x⁴");
                }
                if (b != 0)
                {
                    string sign = b > 0 ? " + " : " - ";
                    string bStr = (Math.Abs(b) == 1) ? "" : FormatNumber(Math.Abs(b));
                    parts.Add($"{sign}{bStr}x³");
                }
                if (c != 0)
                {
                    string sign = c > 0 ? " + " : " - ";
                    string cStr = (Math.Abs(c) == 1) ? "" : FormatNumber(Math.Abs(c));
                    parts.Add($"{sign}{cStr}x²");
                }
                if (d != 0)
                {
                    string sign = d > 0 ? " + " : " - ";
                    string dStr = (Math.Abs(d) == 1) ? "" : FormatNumber(Math.Abs(d));
                    parts.Add($"{sign}{dStr}x");
                }
                if (e != 0)
                {
                    string sign = e > 0 ? " + " : " - ";
                    string eStr = FormatNumber(Math.Abs(e));
                    parts.Add($"{sign}{eStr}");
                }
                parts.Add(" = 0");
                return string.Join("", parts);
            }

            return $"{a}x + {b} = 0";
        }

        private string FormatNumber(double value)
        {
            if (value == (int)value)
                return ((int)value).ToString();
            return value.ToString("0.###").Replace(",", ".");
        }
       
        // ========== КВАДРАТНЫЕ УРАВНЕНИЯ (2 степень) ==========
        [HttpGet]
        public IActionResult Quadratic()
        {
            HttpContext.Session.Remove("QuadraticSession");
            return View("QuadraticInput");
        }

        [HttpPost]
        public IActionResult QuadraticStart(double a, double b, double c)
        {
            var session = new StepByStepSession
            {
                CurrentStep = 1,
                A = a,
                B = b,
                C = c,
                Equation = FormatEquation(a, b, c, degree: 2),
                Degree = 2,
                IsCompleted = false
            };

            HttpContext.Session.SetString("QuadraticSession", JsonSerializer.Serialize(session));

            return RedirectToAction("QuadraticStep1");
        }

        [HttpGet]
        public IActionResult QuadraticStep1()
        {
            var sessionJson = HttpContext.Session.GetString("QuadraticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quadratic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;
            ViewBag.A = session.A;
            ViewBag.B = session.B;
            ViewBag.C = session.C;

            return View(session);
        }

        [HttpPost]
        public IActionResult QuadraticStep1(double userDiscriminant)
        {
            var sessionJson = HttpContext.Session.GetString("QuadraticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quadratic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            double correctD = session.B * session.B - 4 * session.A * session.C;
            session.D = correctD;

            if (Math.Abs(userDiscriminant - correctD) < 0.01)
            {
                session.CurrentStep = 2;
                HttpContext.Session.SetString("QuadraticSession", JsonSerializer.Serialize(session));
                return RedirectToAction("QuadraticStep2");
            }
            else
            {
                ViewBag.Error = $"❌ Неправильно. D = {session.B}² - 4·{session.A}·{session.C} = {correctD}";
                ViewBag.UserAnswer = userDiscriminant;
                ViewBag.Equation = session.Equation;
                ViewBag.A = session.A;
                ViewBag.B = session.B;
                ViewBag.C = session.C;
                return View(session);
            }
        }

        [HttpGet]
        public IActionResult QuadraticStep2()
        {
            var sessionJson = HttpContext.Session.GetString("QuadraticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quadratic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;
            ViewBag.D = session.D;
            ViewBag.A = session.A;
            ViewBag.B = session.B;

            return View(session);
        }

        [HttpPost]
        public IActionResult QuadraticStep2(double root1, double root2)
        {
            var sessionJson = HttpContext.Session.GetString("QuadraticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quadratic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            double sqrtD = Math.Sqrt(session.D);
            double correctRoot1 = (-session.B + sqrtD) / (2 * session.A);
            double correctRoot2 = (-session.B - sqrtD) / (2 * session.A);

            var userRoots = new[] { root1, root2 }.OrderBy(x => x).ToList();
            var correctRoots = new[] { correctRoot1, correctRoot2 }.OrderBy(x => x).ToList();

            bool isCorrect = Math.Abs(userRoots[0] - correctRoots[0]) < 0.01 &&
                            Math.Abs(userRoots[1] - correctRoots[1]) < 0.01;

            if (isCorrect || session.D == 0)
            {
                session.Root1 = correctRoot1;
                session.Root2 = correctRoot2;
                session.CurrentStep = 3;
                session.IsCompleted = true;
                HttpContext.Session.SetString("QuadraticSession", JsonSerializer.Serialize(session));
                return RedirectToAction("QuadraticComplete");
            }
            else
            {
                ViewBag.Error = $"❌ Неправильно. Правильные корни: x₁ = {correctRoot1:F4}, x₂ = {correctRoot2:F4}";
                ViewBag.UserRoot1 = root1;
                ViewBag.UserRoot2 = root2;
                ViewBag.Equation = session.Equation;
                ViewBag.D = session.D;
                ViewBag.A = session.A;
                ViewBag.B = session.B;
                return View(session);
            }
        }

        [HttpGet]
        public IActionResult QuadraticComplete()
        {
            var sessionJson = HttpContext.Session.GetString("QuadraticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quadratic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;
            ViewBag.D = session.D;
            ViewBag.Root1 = session.Root1;
            ViewBag.Root2 = session.Root2;
            ViewBag.A = session.A;
            ViewBag.B = session.B;

            return View(session);
        }

        // ========== ЛИНЕЙНЫЕ УРАВНЕНИЯ (1 степень) ==========
        [HttpGet]
        public IActionResult Linear()
        {
            return View("LinearInput");
        }

        [HttpPost]
        public IActionResult LinearStart(double a, double b)
        {
            var session = new StepByStepSession
            {
                CurrentStep = 1,
                A = a,
                B = b,
                Equation = FormatEquation(a, b, degree: 1),
                Degree = 1,
                IsCompleted = false
            };

            HttpContext.Session.SetString("LinearSession", JsonSerializer.Serialize(session));
            return RedirectToAction("LinearStep1");
        }

        [HttpGet]
        public IActionResult LinearStep1()
        {
            var sessionJson = HttpContext.Session.GetString("LinearSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Linear");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;
            ViewBag.A = session.A;
            ViewBag.B = session.B;

            return View(session);
        }

        [HttpPost]
        public IActionResult LinearStep1(double userRoot)
        {
            var sessionJson = HttpContext.Session.GetString("LinearSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Linear");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            double correctRoot = -session.B / session.A;

            if (Math.Abs(userRoot - correctRoot) < 0.01)
            {
                session.Root1 = correctRoot;
                session.IsCompleted = true;
                HttpContext.Session.SetString("LinearSession", JsonSerializer.Serialize(session));
                return RedirectToAction("LinearComplete");
            }
            else
            {
                ViewBag.Error = $"❌ Неправильно. x = -b/a = -{session.B}/{session.A} = {correctRoot:F4}";
                ViewBag.UserAnswer = userRoot;
                ViewBag.Equation = session.Equation;
                ViewBag.A = session.A;
                ViewBag.B = session.B;
                return View(session);
            }
        }

        [HttpGet]
        public IActionResult LinearComplete()
        {
            var sessionJson = HttpContext.Session.GetString("LinearSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Linear");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;
            ViewBag.Root1 = session.Root1;
            ViewBag.A = session.A;
            ViewBag.B = session.B;

            return View(session);
        }


        //  КУБИЧЕСКИЕ УРАВНЕНИЯ (3 степень)
        [HttpGet]
        public IActionResult Cubic()
        {
            HttpContext.Session.Remove("CubicSession");
            return View("CubicInput");
        }

        [HttpPost]
        public IActionResult CubicStart(double a, double b, double c, double d)
        {
            var session = new StepByStepSession
            {
                CurrentStep = 1,
                A = a,
                B = b,
                C = c,
                D = d,
                Equation = FormatEquation(a, b, c, d, degree: 3),
                Degree = 3,
                IsCompleted = false
            };

            HttpContext.Session.SetString("CubicSession", JsonSerializer.Serialize(session));
            return RedirectToAction("CubicStep1");
        }

        [HttpGet]
        public IActionResult CubicStep1()
        {
            var sessionJson = HttpContext.Session.GetString("CubicSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Cubic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;
            ViewBag.A = session.A;
            ViewBag.B = session.B;
            ViewBag.C = session.C;
            ViewBag.D = session.D;

            // Вычисляем промежуточные значения для отображения
            double numeratorP = 3 * session.A * session.C - session.B * session.B;
            double denominatorP = 3 * session.A * session.A;
            double numeratorQ = 2 * Math.Pow(session.B, 3) - 9 * session.A * session.B * session.C + 27 * Math.Pow(session.A, 2) * session.D;
            double denominatorQ = 27 * Math.Pow(session.A, 3);

            ViewBag.NumeratorP = numeratorP;
            ViewBag.DenominatorP = denominatorP;
            ViewBag.NumeratorQ = numeratorQ;
            ViewBag.DenominatorQ = denominatorQ;

            return View(session);
        }

        [HttpPost]
        public IActionResult CubicStep1(double userP, double userQ)
        {
            var sessionJson = HttpContext.Session.GetString("CubicSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Cubic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            ViewBag.Equation = session.Equation;
            ViewBag.A = session.A;
            ViewBag.B = session.B;
            ViewBag.C = session.C;
            ViewBag.D = session.D;

            // Вычисляем p и q (приведённое уравнение t³ + pt + q = 0)
            double shift = session.B / (3 * session.A);
            double p = (3 * session.A * session.C - session.B * session.B) / (3 * session.A * session.A);
            double q = (2 * session.B * session.B * session.B - 9 * session.A * session.B * session.C + 27 * session.A * session.A * session.D)
                       / (27 * session.A * session.A * session.A);

            session.P = p;
            session.Q = q;
            session.Shift = shift;

            // Проверяем ответ пользователя
            bool pCorrect = Math.Abs(userP - p) < 0.01;
            bool qCorrect = Math.Abs(userQ - q) < 0.01;

            if (pCorrect && qCorrect)
            {
                session.CurrentStep = 2;
                HttpContext.Session.SetString("CubicSession", JsonSerializer.Serialize(session));
                return RedirectToAction("CubicStep2");
            }
            else
            {
                if (!pCorrect) ViewBag.ErrorP = $"p = {p:F4}";
                if (!qCorrect) ViewBag.ErrorQ = $"q = {q:F4}";
                ViewBag.Equation = session.Equation;
                ViewBag.A = session.A;
                ViewBag.B = session.B;
                ViewBag.C = session.C;
                ViewBag.D = session.D;
                ViewBag.UserP = userP;
                ViewBag.UserQ = userQ;
                return View(session);
            }
        }

        [HttpGet]
        public IActionResult CubicStep2()
        {
            var sessionJson = HttpContext.Session.GetString("CubicSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Cubic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            // Вычисляем дискриминант
            double delta = Math.Pow(session.Q / 2, 2) + Math.Pow(session.P / 3, 3);
            session.Delta = delta;

            // ОТЛАДКА: выводим в консоль Visual Studio
            System.Diagnostics.Debug.WriteLine($"========== CUBIC STEP 2 ==========");
            System.Diagnostics.Debug.WriteLine($"p = {session.P}");
            System.Diagnostics.Debug.WriteLine($"q = {session.Q}");
            System.Diagnostics.Debug.WriteLine($"delta = {delta}");
            System.Diagnostics.Debug.WriteLine($"==================================");

            ViewBag.Equation = session.Equation;
            ViewBag.P = session.P;
            ViewBag.Q = session.Q;
            ViewBag.Delta = delta;

            return View(session);
        }

        /* [HttpGet]
         public IActionResult CubicStep2()
         {
             var sessionJson = HttpContext.Session.GetString("CubicSession");
             if (string.IsNullOrEmpty(sessionJson))
                 return RedirectToAction("Cubic");

             var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
             ViewBag.Equation = session.Equation;
             ViewBag.P = session.P;
             ViewBag.Q = session.Q;



             // Вычисляем Delta
             double delta = Math.Pow(session.Q / 2, 2) + Math.Pow(session.P / 3, 3);
             session.Delta = delta;
             ViewBag.Delta = delta;

             System.Diagnostics.Debug.WriteLine($"CubicStep2: p={session.P}, q={session.Q}, delta={delta}");

             return View(session);
         }*/

        [HttpPost]
        public IActionResult CubicStep2(string userDelta)
        {
            var sessionJson = HttpContext.Session.GetString("CubicSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Cubic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            double delta = Math.Pow(session.Q / 2, 2) + Math.Pow(session.P / 3, 3);

            // Парсим введённое значение
            double parsedUserDelta;
            try
            {
                string normalized = userDelta?.Trim().Replace(',', '.') ?? "0";
                parsedUserDelta = double.Parse(normalized, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                ViewBag.Error = "❌ Неверный формат числа.";
                ViewBag.UserDelta = userDelta;
                ViewBag.Equation = session.Equation;
                ViewBag.P = session.P;
                ViewBag.Q = session.Q;
                ViewBag.Delta = delta;
                return View(session);
            }

            // СОХРАНЯЕМ Delta В СЕССИЮ
            session.Delta = delta;

            if (Math.Abs(parsedUserDelta - delta) < 0.001)
            {
                session.CurrentStep = 3;
                HttpContext.Session.SetString("CubicSession", JsonSerializer.Serialize(session));
                return RedirectToAction("CubicStep3");
            }
            else
            {
                ViewBag.Error = $"❌ Неправильно. Δ = {delta:F4}";
                ViewBag.UserDelta = userDelta;
                ViewBag.Equation = session.Equation;
                ViewBag.P = session.P;
                ViewBag.Q = session.Q;
                ViewBag.Delta = delta;
                return View(session);
            }
        }

        [HttpGet]
        public IActionResult CubicStep3()
        {
            var sessionJson = HttpContext.Session.GetString("CubicSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Cubic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;

            // ИСПОЛЬЗУЕМ Delta ИЗ СЕССИИ (НЕ ПЕРЕСЧИТЫВАЕМ!)
            double delta = session.Delta;
            ViewBag.Delta = delta;
            ViewBag.P = session.P;
            ViewBag.Q = session.Q;

            // Вычисляем корни
            if (delta > 0)
            {
                double sqrtDelta = Math.Sqrt(delta);
                double alpha = Math.Cbrt(-session.Q / 2 + sqrtDelta);
                double beta = Math.Cbrt(-session.Q / 2 - sqrtDelta);
                session.Alpha = alpha;
                session.Beta = beta;

                double t1 = alpha + beta;
                session.Root1 = t1 - session.Shift;
                session.Root2Real = -(alpha + beta) / 2;
                session.Root2Imag = (alpha - beta) * Math.Sqrt(3) / 2;
            }
            else if (Math.Abs(delta) < 0.0001)
            {
                double u = Math.Cbrt(-session.Q / 2);
                double t1 = 2 * u;
                double t2 = -u;
                session.Root1 = t1 - session.Shift;
                session.Root2 = t2 - session.Shift;
                session.Root3 = t2 - session.Shift;
            }
            else
            {
                // delta < 0
                if (Math.Abs(session.Q) < 0.0001)
                {
                    double t1 = 0;
                    double t2 = Math.Sqrt(-session.P);
                    double t3 = -Math.Sqrt(-session.P);

                    session.Root1 = t1 - session.Shift;
                    session.Root2 = t2 - session.Shift;
                    session.Root3 = t3 - session.Shift;
                }
                else
                {
                    double r = 2 * Math.Sqrt(-session.P / 3);
                    double phi = Math.Acos((3 * session.Q) / (2 * session.P) * Math.Sqrt(-3 / session.P));

                    double t1 = r * Math.Cos(phi / 3);
                    double t2 = r * Math.Cos((phi + 2 * Math.PI) / 3);
                    double t3 = r * Math.Cos((phi + 4 * Math.PI) / 3);

                    session.Root1 = t1 - session.Shift;
                    session.Root2 = t2 - session.Shift;
                    session.Root3 = t3 - session.Shift;
                }
            }

            System.Diagnostics.Debug.WriteLine($"=== CUBIC STEP 3 ОТЛАДКА ===");
            System.Diagnostics.Debug.WriteLine($"P = {session.P}");
            System.Diagnostics.Debug.WriteLine($"Q = {session.Q}");
            System.Diagnostics.Debug.WriteLine($"Delta = {delta}");
            System.Diagnostics.Debug.WriteLine($"Shift = {session.Shift}");
            System.Diagnostics.Debug.WriteLine($"Root1 = {session.Root1}");
            System.Diagnostics.Debug.WriteLine($"Root2 = {session.Root2}");
            System.Diagnostics.Debug.WriteLine($"Root3 = {session.Root3}");

            session.CurrentStep = 4;
            session.IsCompleted = true;
            HttpContext.Session.SetString("CubicSession", JsonSerializer.Serialize(session));

            return RedirectToAction("CubicComplete");
        }

        [HttpGet]
        public IActionResult CubicComplete()
        {
            var sessionJson = HttpContext.Session.GetString("CubicSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Cubic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;
            ViewBag.Delta = session.Delta;
            ViewBag.Root1 = session.Root1;
            ViewBag.Root2 = session.Root2;
            ViewBag.Root3 = session.Root3;
            ViewBag.Shift = session.Shift;
            ViewBag.P = session.P;
            ViewBag.Q = session.Q;

            System.Diagnostics.Debug.WriteLine($"=== CUBIC COMPLETE GET ===");
            System.Diagnostics.Debug.WriteLine($"Root1 = {session.Root1}");
            System.Diagnostics.Debug.WriteLine($"Root2 = {session.Root2}");
            System.Diagnostics.Debug.WriteLine($"Root3 = {session.Root3}");

            return View(session);
        }


        [HttpGet]
        // ========== УРАВНЕНИЯ 4 СТЕПЕНИ (метод Феррари) ==========
        [HttpGet]
        public IActionResult Quartic()
        {
            HttpContext.Session.Remove("QuarticSession");
            return View("QuarticInput");
        }

        [HttpPost]
        public IActionResult QuarticStart(double a, double b, double c, double d, double e)
        {
            var session = new StepByStepSession
            {
                CurrentStep = 1,
                A = a,
                B = b,
                C = c,
                D = d,
                E = e,
                Equation = FormatEquation(a, b, c, d, e, degree: 4),
                Degree = 4,
                IsCompleted = false
            };

            HttpContext.Session.SetString("QuarticSession", JsonSerializer.Serialize(session));
            return RedirectToAction("QuarticStep1");
        }

        [HttpGet]
        public IActionResult QuarticStep1()
        {
            var sessionJson = HttpContext.Session.GetString("QuarticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quartic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;
            ViewBag.A = session.A;
            ViewBag.B = session.B;
            ViewBag.C = session.C;
            ViewBag.D = session.D;
            ViewBag.E = session.E;

            return View(session);
        }

        [HttpPost]
        public IActionResult QuarticStep1(double userP, double userQ, double userR)
        {
            var sessionJson = HttpContext.Session.GetString("QuarticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quartic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            // Замена x = y - b/(4a)
            double shift = session.B / (4 * session.A);
            double p = session.C / session.A - 3 * session.B * session.B / (8 * session.A * session.A);
            double q = session.D / session.A - session.B * session.C / (2 * session.A * session.A) + session.B * session.B * session.B / (8 * session.A * session.A * session.A);
            double r = session.E / session.A - session.B * session.D / (4 * session.A * session.A) + session.B * session.B * session.C / (16 * session.A * session.A * session.A) - 3 * session.B * session.B * session.B * session.B / (256 * session.A * session.A * session.A * session.A);

            session.Shift = shift;
            session.P1 = p;
            session.Q1 = q;
            session.R1 = r;

            bool pCorrect = Math.Abs(userP - p) < 0.01;
            bool qCorrect = Math.Abs(userQ - q) < 0.01;
            bool rCorrect = Math.Abs(userR - r) < 0.01;

            if (pCorrect && qCorrect && rCorrect)
            {
                session.CurrentStep = 2;
                HttpContext.Session.SetString("QuarticSession", JsonSerializer.Serialize(session));
                return RedirectToAction("QuarticStep2");
            }
            else
            {
                if (!pCorrect) ViewBag.ErrorP = $"p = {p:F4}";
                if (!qCorrect) ViewBag.ErrorQ = $"q = {q:F4}";
                if (!rCorrect) ViewBag.ErrorR = $"r = {r:F4}";
                ViewBag.Equation = session.Equation;
                ViewBag.A = session.A;
                ViewBag.B = session.B;
                ViewBag.C = session.C;
                ViewBag.D = session.D;
                ViewBag.E = session.E;
                ViewBag.UserP = userP;
                ViewBag.UserQ = userQ;
                ViewBag.UserR = userR;
                return View(session);
            }
        }

        [HttpGet]
        public IActionResult QuarticStep2()
        {
            var sessionJson = HttpContext.Session.GetString("QuarticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quartic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.P1 = session.P1;
            ViewBag.Q1 = session.Q1;
            ViewBag.R1 = session.R1;

            return View(session);
        }

        [HttpPost]
        public IActionResult QuarticStep2(double userT0)
        {
            var sessionJson = HttpContext.Session.GetString("QuarticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quartic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            // Кубическая резольвента: t³ - p t² - 4r t + (4pr - q²) = 0
            // Для простоты используем первый действительный корень
            // Это упрощённый вариант — для полной реализации нужен CubicSolver

            double t0 = session.P1;  // временное упрощение

            session.T0 = t0;

            if (Math.Abs(userT0 - t0) < 0.01)
            {
                session.CurrentStep = 3;
                HttpContext.Session.SetString("QuarticSession", JsonSerializer.Serialize(session));
                return RedirectToAction("QuarticStep3");
            }
            else
            {
                ViewBag.Error = $"t₀ = {t0:F4}";
                ViewBag.UserT0 = userT0;
                return View(session);
            }
        }


        [HttpGet]
        public IActionResult QuarticStep3()
        {
            var sessionJson = HttpContext.Session.GetString("QuarticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quartic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            double A = Math.Sqrt(Math.Abs(session.T0));
            double B = session.Q1 / (2 * A);
            double C = (session.P1 + session.T0) / 2;

            session.A1 = A;
            session.B1 = B;
            session.C1 = C;

            ViewBag.T0 = session.T0;
            ViewBag.A1 = A;
            ViewBag.B1 = B;
            ViewBag.C1 = C;
            ViewBag.CMinusB = C - B;
            ViewBag.CPlusB = C + B;

            return View(session);
        }

        [HttpPost]
        public IActionResult QuarticStep3(string next)
        {
            var sessionJson = HttpContext.Session.GetString("QuarticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quartic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);

            // Решаем два квадратных уравнения
            double A = session.A1;
            double B = session.B1;
            double C = session.C1;

            // Первое квадратное уравнение: y² + Ay + (C - B) = 0
            double disc1 = A * A - 4 * (C - B);
            if (disc1 >= 0)
            {
                session.Root1 = (-A + Math.Sqrt(disc1)) / 2 - session.Shift;
                session.Root2 = (-A - Math.Sqrt(disc1)) / 2 - session.Shift;
            }
            else
            {
                session.Root1 = (-A / 2) - session.Shift;
                session.Root2Real = (-A / 2);
                session.Root2Imag = Math.Sqrt(-disc1) / 2;
            }

            // Второе квадратное уравнение: y² - Ay + (C + B) = 0
            double disc2 = A * A - 4 * (C + B);
            if (disc2 >= 0)
            {
                session.Root3 = (A + Math.Sqrt(disc2)) / 2 - session.Shift;
                session.Root4 = (A - Math.Sqrt(disc2)) / 2 - session.Shift;
            }
            else
            {
                session.Root3Real = (A / 2) - session.Shift;
                session.Root3Imag = Math.Sqrt(-disc2) / 2;
            }

            session.CurrentStep = 4;
            session.IsCompleted = true;
            HttpContext.Session.SetString("QuarticSession", JsonSerializer.Serialize(session));

            return RedirectToAction("QuarticComplete");
        }

        [HttpGet]
        public IActionResult QuarticComplete()
        {
            var sessionJson = HttpContext.Session.GetString("QuarticSession");
            if (string.IsNullOrEmpty(sessionJson))
                return RedirectToAction("Quartic");

            var session = JsonSerializer.Deserialize<StepByStepSession>(sessionJson);
            ViewBag.Equation = session.Equation;
            ViewBag.Shift = session.Shift;
            ViewBag.P1 = session.P1;
            ViewBag.Q1 = session.Q1;
            ViewBag.R1 = session.R1;
            ViewBag.T0 = session.T0;
            ViewBag.A1 = session.A1;
            ViewBag.B1 = session.B1;
            ViewBag.C1 = session.C1;
            ViewBag.Root1 = session.Root1;
            ViewBag.Root2 = session.Root2;
            ViewBag.Root3 = session.Root3;
            ViewBag.Root4 = session.Root4;

            return View(session);
        }
    }
}