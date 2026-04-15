using System.Numerics;

namespace Web.Services
{
    public interface IGenerator
    {
        Equation Generate(GenParams p);
    }

    public class Equation
    {
        public double[] Coeffs { get; set; }
        public int Degree => Coeffs.Length - 1;
        public string Text { get; set; }

        public Equation(double[] coeffs)
        {
            Coeffs = coeffs;
            Text = MakeText();
        }

        private string MakeText()
        {
            int n = Degree;
            var parts = new List<string>();
            for (int i = 0; i < Coeffs.Length; i++)
            {
                double c = Coeffs[i];
                if (Math.Abs(c) < 1e-10) continue;

                int power = n - i;
                string sign = c > 0 ? (i == 0 ? "" : "+") : "-";
                string val = (Math.Abs(c) == 1 && power > 0) ? "" : Math.Abs(c).ToString();
                string varPart = power == 0 ? "" : (power == 1 ? "x" : $"x^{power}");
                parts.Add($"{sign}{val}{varPart}");
            }
            if (parts.Count == 0) return "0 = 0";
            return string.Join("", parts) + " = 0";
        }
    }

    public class Result
    {
        public List<Complex> Roots { get; set; }
        public List<string> Steps { get; set; }
        public string Error { get; set; }
        
        public Result()
        {
            Roots = new List<Complex>();
            Steps = new List<string>();
            Error = "";
        }
        
        public bool Success => string.IsNullOrEmpty(Error);
    }

    public class GenParams
    {
        public int Degree { get; set; }
        public bool IntegerRoots { get; set; }
        public bool RealOnly { get; set; }
    }

    public abstract class Solver
    {
        public abstract int Degree { get; }
        public abstract string Name { get; }
        public abstract string Help { get; }
        public abstract Result Solve(Equation eq);

        private bool _instruction = false;

        protected void PrintStep(Result res, string step)
        {
            if (!_instruction)
            {
                Console.WriteLine("\n=== нажимайте любую клавишу для следующего шага ===");
                _instruction = true;
            }
            res.Steps.Add(step);
           /* Console.WriteLine(step);
            Console.ReadKey();*/
        }
    }

    public static class Fmt
    {
        public static string Num(double x)
        {
            if (Math.Abs(x - Math.Round(x)) < 1e-10)
                return ((int)Math.Round(x)).ToString();

            double bestErr = 1.0;
            int bestN = 0, bestD = 1;
            for (int d = 1; d <= 100; d++)
            {
                int n = (int)Math.Round(x * d);
                double err = Math.Abs(x - (double)n / d);
                if (err < bestErr)
                {
                    bestErr = err;
                    bestN = n;
                    bestD = d;
                }
                if (bestErr < 1e-8) break;
            }
            return bestD == 1 ? bestN.ToString() : $"{bestN}/{bestD}";
        }
    }

    
    public class LinearSolver : Solver
    {
        public override int Degree => 1;
        public override string Name => "Линейное уравнение";
        public override string Help => "x = -b/a";

        public override Result Solve(Equation eq)
        {
            Result res = new Result();
            double a = eq.Coeffs[0];
            double b = eq.Coeffs[1];

            PrintStep(res, $"Уравнение: {eq.Text}");
            PrintStep(res, $"a = {Fmt.Num(a)}, b = {Fmt.Num(b)}");
            PrintStep(res, $"{Fmt.Num(a)}x + {Fmt.Num(b)} = 0");

            if (Math.Abs(a) < 1e-10)
            {
                res.Error = Math.Abs(b) < 1e-10 ? "0=0 → бесконечно много решений" : "решений нет";
                return res;
            }

            double x = -b / a;
            PrintStep(res, $"x = {Fmt.Num(-b)} / {Fmt.Num(a)} = {Fmt.Num(x)}");
            res.Roots.Add(new Complex(x, 0));
            return res;
        }
    }

    public class LinearGen : IGenerator
    {
        Random rnd = new Random();
        
        public Equation Generate(GenParams p)
        {
            if (p.IntegerRoots)
            {
                int root = rnd.Next(-15, 16);
                double a = rnd.Next(1, 10) * (rnd.Next(0, 2) == 0 ? 1 : -1);
                double b = -a * root;
                return new Equation(new double[] { a, b });
            }
            else
            {
                double a = rnd.Next(1, 11) * (rnd.Next(0, 2) == 0 ? 1 : -1);
                double b = rnd.Next(-10, 11);
                return new Equation(new double[] { a, b });
            }
        }
    }

        public class QuadSolver : Solver
    {
        public override int Degree => 2;
        public override string Name => "Дискриминант";
        public override string Help => "D = b² - 4ac, x = (-b ± √D)/(2a)";

        public override Result Solve(Equation eq)
        {
            Result res = new Result();
            double a = eq.Coeffs[0];
            double b = eq.Coeffs[1];
            double c = eq.Coeffs[2];

            PrintStep(res, $"Уравнение: {eq.Text}");
            PrintStep(res, $"a = {Fmt.Num(a)}, b = {Fmt.Num(b)}, c = {Fmt.Num(c)}");
            
            double d = b * b - 4 * a * c;
            PrintStep(res, $"D = {Fmt.Num(b)}² - 4·{Fmt.Num(a)}·{Fmt.Num(c)} = {Fmt.Num(d)}");

            if (d > 0)
            {
                PrintStep(res, "D > 0 → два действительных корня");
                double sqrtD = Math.Sqrt(d);
                double x1 = (-b + sqrtD) / (2 * a);
                double x2 = (-b - sqrtD) / (2 * a);
                PrintStep(res, $"x₁ = ({Fmt.Num(-b)} + √{Fmt.Num(d)}) / {Fmt.Num(2 * a)} = {Fmt.Num(x1)}");
                PrintStep(res, $"x₂ = ({Fmt.Num(-b)} - √{Fmt.Num(d)}) / {Fmt.Num(2 * a)} = {Fmt.Num(x2)}");
                res.Roots.Add(new Complex(x1, 0));
                res.Roots.Add(new Complex(x2, 0));
            }
            else if (Math.Abs(d) < 1e-10)
            {
                PrintStep(res, "D = 0 → один корень");
                double x = -b / (2 * a);
                PrintStep(res, $"x = {Fmt.Num(-b)} / {Fmt.Num(2 * a)} = {Fmt.Num(x)}");
                res.Roots.Add(new Complex(x, 0));
            }
            else
            {
                PrintStep(res, "D < 0 → два комплексных корня");
                double sqrtD = Math.Sqrt(-d);
                double re = -b / (2 * a);
                double im = sqrtD / (2 * a);
                PrintStep(res, $"x₁ = {Fmt.Num(re)} + {Fmt.Num(im)}i");
                PrintStep(res, $"x₂ = {Fmt.Num(re)} - {Fmt.Num(im)}i");
                res.Roots.Add(new Complex(re, im));
                res.Roots.Add(new Complex(re, -im));
            }
            return res;
        }
    }

    public class QuadGen : IGenerator
    {
        Random rnd = new Random();
        
        public Equation Generate(GenParams p)
        {
            double a = 1;
            
            
            if (!p.RealOnly)
            {
                double b = rnd.Next(-15, 16);
                double c = rnd.Next(-15, 16);
                while (b * b - 4 * a * c >= 0)
                {
                    b = rnd.Next(-15, 16);
                    c = rnd.Next(-15, 16);
                }
                return new Equation(new double[] { a, b, c });
            }
            
            // вещественные корни
            if (p.IntegerRoots)
            {
                int r1 = rnd.Next(-10, 11);
                int r2 = rnd.Next(-10, 11);
                return new Equation(new double[] { 1, -(r1 + r2), r1 * r2 });
            }
            else
            {
                double b = rnd.Next(-15, 16);
                double c = rnd.Next(-15, 16);
                while (b * b - 4 * a * c < 0)
                {
                    b = rnd.Next(-15, 16);
                    c = rnd.Next(-15, 16);
                }
                return new Equation(new double[] { a, b, c });
            }
        }
    }

    
    public class CubicSolver : Solver
    {
        public override int Degree => 3;
        public override string Name => "Метод Кардано";
        public override string Help => "t³ + pt + q = 0, Δ = (q/2)² + (p/3)³";

        public override Result Solve(Equation eq)
        {
            Result res = new Result();
            double[] c = eq.Coeffs;
            double a = c[0], b = c[1], cc = c[2], d = c[3];

            PrintStep(res, $"Уравнение: {eq.Text}");
            PrintStep(res, $"a={Fmt.Num(a)}, b={Fmt.Num(b)}, c={Fmt.Num(cc)}, d={Fmt.Num(d)}");

            double shift = b / (3 * a);
            PrintStep(res, $"Замена x = t - {Fmt.Num(shift)}");

            double p = (3 * a * cc - b * b) / (3 * a * a);
            double q = (2 * b * b * b - 9 * a * b * cc + 27 * a * a * d) / (27 * a * a * a);
            PrintStep(res, $"p = {Fmt.Num(p)}, q = {Fmt.Num(q)}");
            PrintStep(res, $"t³ + {Fmt.Num(p)}t + {Fmt.Num(q)} = 0");

            double delta = Math.Pow(q / 2, 2) + Math.Pow(p / 3, 3);
            PrintStep(res, $"Δ = (q/2)² + (p/3)³ = {Fmt.Num(delta)}");

            if (delta > 0)
            {
                PrintStep(res, "Δ > 0 → один действительный + два комплексных");
                double sqrtDelta = Math.Sqrt(delta);
                double alpha = Math.Cbrt(-q / 2 + sqrtDelta);
                double beta = Math.Cbrt(-q / 2 - sqrtDelta);
                PrintStep(res, $"α = ∛({Fmt.Num(-q / 2)} + √{Fmt.Num(delta)}) = {Fmt.Num(alpha)}");
                PrintStep(res, $"β = ∛({Fmt.Num(-q / 2)} - √{Fmt.Num(delta)}) = {Fmt.Num(beta)}");
                
                double t1 = alpha + beta;
                PrintStep(res, $"t₁ = α + β = {Fmt.Num(t1)}");
                res.Roots.Add(new Complex(t1 - shift, 0));

                double realPart = -(alpha + beta) / 2;
                double imagPart = (alpha - beta) * Math.Sqrt(3) / 2;
                PrintStep(res, $"t₂ = {Fmt.Num(realPart)} + {Fmt.Num(imagPart)}i");
                PrintStep(res, $"t₃ = {Fmt.Num(realPart)} - {Fmt.Num(imagPart)}i");
                res.Roots.Add(new Complex(realPart - shift, imagPart));
                res.Roots.Add(new Complex(realPart - shift, -imagPart));
            }
            else if (Math.Abs(delta) < 1e-10)
            {
                PrintStep(res, "Δ = 0 → кратные корни");
                double u = Math.Cbrt(-q / 2);
                double t1 = 2 * u;
                double t2 = -u;
                PrintStep(res, $"t₁ = {Fmt.Num(t1)}");
                PrintStep(res, $"t₂ = t₃ = {Fmt.Num(t2)}");
                res.Roots.Add(new Complex(t1 - shift, 0));
                res.Roots.Add(new Complex(t2 - shift, 0));
                res.Roots.Add(new Complex(t2 - shift, 0));
            }
            else
            {
                PrintStep(res, "Δ < 0 → три действительных корня");
                double r = 2 * Math.Sqrt(-p / 3);
                double phi = Math.Acos((3 * q) / (2 * p) * Math.Sqrt(-3 / p));
                double t1 = r * Math.Cos(phi / 3);
                double t2 = r * Math.Cos((phi + 2 * Math.PI) / 3);
                double t3 = r * Math.Cos((phi + 4 * Math.PI) / 3);
                PrintStep(res, $"t₁ = {Fmt.Num(t1)}");
                PrintStep(res, $"t₂ = {Fmt.Num(t2)}");
                PrintStep(res, $"t₃ = {Fmt.Num(t3)}");
                res.Roots.Add(new Complex(t1 - shift, 0));
                res.Roots.Add(new Complex(t2 - shift, 0));
                res.Roots.Add(new Complex(t3 - shift, 0));
            }
            return res;
        }
    }

    public class CubicGen : IGenerator
    {
        Random rnd = new Random();
        
        public Equation Generate(GenParams p)
        {
            if (p.IntegerRoots && p.RealOnly)
            {
                int r1 = rnd.Next(-10, 11);
                int r2 = rnd.Next(-10, 11);
                int r3 = rnd.Next(-10, 11);
                return new Equation(new double[] { 1, -(r1 + r2 + r3), r1 * r2 + r1 * r3 + r2 * r3, -r1 * r2 * r3 });
            }
            else if (p.IntegerRoots && !p.RealOnly)
            {
                // один целый корень + квадратный трёхчлен с D < 0
                int r1 = rnd.Next(-10, 11);
                double a2 = 1;
                double b2 = rnd.Next(-15, 16);
                double c2 = rnd.Next(-15, 16);
                while (b2 * b2 - 4 * a2 * c2 >= 0)
                {
                    b2 = rnd.Next(-15, 16);
                    c2 = rnd.Next(-155, 16);
                }
                double b = -r1 + b2;
                double c = -r1 * b2 + c2;
                double d = -r1 * c2;
                return new Equation(new double[] { 1, b, c, d });
            }
            else
            {
                double b = rnd.Next(-15, 16);
                double c = rnd.Next(-15, 16);
                double d = rnd.Next(-15, 16);
                return new Equation(new double[] { 1, b, c, d });
            }
        }
    }

    
    public class QuarticSolver : Solver
    {
        public override int Degree => 4;
        public override string Name => "Метод Феррари";
        public override string Help => "Сведение к кубической резольвенте";

        public override Result Solve(Equation eq)
        {
            Result res = new Result();
            double[] c = eq.Coeffs;
            double a = c[0], b = c[1], cc = c[2], d = c[3], e = c[4];

            PrintStep(res, $"Уравнение: {eq.Text}");
            PrintStep(res, $"a={Fmt.Num(a)}, b={Fmt.Num(b)}, c={Fmt.Num(cc)}, d={Fmt.Num(d)}, e={Fmt.Num(e)}");

            double shift = b / (4 * a);
            if (Math.Abs(shift) > 1e-10)
                PrintStep(res, shift > 0 ? $"Замена x = y - {Fmt.Num(shift)}" : $"Замена x = y + {Fmt.Num(-shift)}");
            else
                PrintStep(res, "Кубический член отсутствует");

            double p = cc / a - 3 * b * b / (8 * a * a);
            double q = d / a - b * cc / (2 * a * a) + b * b * b / (8 * a * a * a);
            double r = e / a - b * d / (4 * a * a) + b * b * cc / (16 * a * a * a) - 3 * b * b * b * b / (256 * a * a * a * a);
            PrintStep(res, $"p = {Fmt.Num(p)}, q = {Fmt.Num(q)}, r = {Fmt.Num(r)}");

            double[] cubicCoeffs = { 1, -p, -4 * r, 4 * p * r - q * q };
            CubicSolver cubicSolver = new CubicSolver();
            Result cubicRes = cubicSolver.Solve(new Equation(cubicCoeffs));

            if (!cubicRes.Success || cubicRes.Roots.Count == 0)
            {
                res.Error = "не удалось решить кубическую резольвенту";
                return res;
            }

            double t = cubicRes.Roots[0].Real;
            PrintStep(res, $"t = {Fmt.Num(t)}");

            double A = Math.Sqrt(Math.Abs(t));
            double B = q / (2 * A);
            double C = (p + t) / 2;

            PrintStep(res, $"A = {Fmt.Num(A)}, B = {Fmt.Num(B)}, C = {Fmt.Num(C)}");
            PrintStep(res, $"Разложение: (y² + {Fmt.Num(A)}y + {Fmt.Num(C - B)})(y² - {Fmt.Num(A)}y + {Fmt.Num(C + B)}) = 0");

            SolveQuad(res, 1, A, C - B, shift);
            SolveQuad(res, 1, -A, C + B, shift);

            return res;
        }

        private void SolveQuad(Result res, double a, double b, double c, double shift)
        {
            double d = b * b - 4 * a * c;
            double sqrtD = Math.Sqrt(Math.Abs(d));
            if (d >= 0)
            {
                double x1 = (-b + sqrtD) / (2 * a);
                double x2 = (-b - sqrtD) / (2 * a);
                PrintStep(res, $"y₁ = {Fmt.Num(x1)}");
                PrintStep(res, $"y₂ = {Fmt.Num(x2)}");
                res.Roots.Add(new Complex(x1 - shift, 0));
                res.Roots.Add(new Complex(x2 - shift, 0));
            }
            else
            {
                double re = -b / (2 * a);
                double im = sqrtD / (2 * a);
                PrintStep(res, $"y₁ = {Fmt.Num(re)} + {Fmt.Num(im)}i");
                PrintStep(res, $"y₂ = {Fmt.Num(re)} - {Fmt.Num(im)}i");
                res.Roots.Add(new Complex(re - shift, im));
                res.Roots.Add(new Complex(re - shift, -im));
            }
        }
    }

    public class QuarticGen : IGenerator
    {
    Random rnd = new Random();
    
    public Equation Generate(GenParams p)
    {
        if (p.IntegerRoots && p.RealOnly)
        {
            int r1 = rnd.Next(-10, 11);
            int r2 = rnd.Next(-10, 11);
            int r3 = rnd.Next(-10, 11);
            int r4 = rnd.Next(-10, 11);
            double b = -(r1 + r2 + r3 + r4);
            double c = r1 * r2 + r1 * r3 + r1 * r4 + r2 * r3 + r2 * r4 + r3 * r4;
            double d = -(r1 * r2 * r3 + r1 * r2 * r4 + r1 * r3 * r4 + r2 * r3 * r4);
            double e = r1 * r2 * r3 * r4;
            return new Equation(new double[] { 1, b, c, d, e });
        }
        else if (!p.RealOnly)
        {
            // два квадратных трёхчлена с D < 0
            double a1 = 1, a2 = 1;
            double b1, c1, b2, c2;
            
            do { b1 = rnd.Next(-15, 16); c1 = rnd.Next(-15, 16); } 
            while (b1 * b1 - 4 * a1 * c1 >= 0);
            
            do { b2 = rnd.Next(-15, 16); c2 = rnd.Next(-15, 16); } 
            while (b2 * b2 - 4 * a2 * c2 >= 0);
            
            double a = 1;
            double b = b1 + b2;
            double c = c1 + c2 + b1 * b2;
            double d = b1 * c2 + b2 * c1;
            double e = c1 * c2;
            
            return new Equation(new double[] { a, b, c, d, e });
        }
        else
        {
            return new Equation(new double[] { 1, rnd.Next(-10, 11), rnd.Next(-10, 11), rnd.Next(-10, 11), rnd.Next(-10, 11) });
        }
    }
    }
    public class EquationResult
    {
        public string equation { get; set; }
        public List<EquationRoot> roots { get; set; }
        public List<string> steps { get; set; }
        public bool success { get; set; }
        public string error { get; set; }
    }

    public class EquationRoot
    {
        public double Real { get; set; }
        public double Imaginary { get; set; }
    }

    // ========== СЕРВИС ДЛЯ КОНТРОЛЛЕРА ==========
    public class EquationSolverService
    {
        public EquationResult SolveEquation(int degree, bool integerRoots, bool realOnly, string equationText)
        {
            System.Diagnostics.Debug.WriteLine($"SolveEquation: degree={degree}, integerRoots={integerRoots}, realOnly={realOnly}");

            try
            {
                var genParams = new GenParams
                {
                    Degree = degree,
                    IntegerRoots = integerRoots,
                    RealOnly = realOnly
                };
                System.Diagnostics.Debug.WriteLine($"GenParams созданы");

                Equation equation = GenerateEquation(degree, genParams);
                System.Diagnostics.Debug.WriteLine($"Уравнение сгенерировано: {equation.Text}");

                Result result = SolveEquation(equation);
                System.Diagnostics.Debug.WriteLine($"Уравнение решено, корней: {result.Roots.Count}, шагов: {result.Steps.Count}");

                var equationResult = new EquationResult
                {
                    equation = equation.Text,
                    roots = result.Roots.Select(r => new EquationRoot { Real = r.Real, Imaginary = r.Imaginary }).ToList(),
                    steps = result.Steps,
                    success = result.Success,
                    error = result.Error
                };
                System.Diagnostics.Debug.WriteLine($"EquationResult создан");

                return equationResult;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ОШИБКА в SolveEquation: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        private Equation GenerateEquation(int degree, GenParams p)
        {
            IGenerator generator = degree switch
            {
                1 => new LinearGen(),
                2 => new QuadGen(),
                3 => new CubicGen(),
                4 => new QuarticGen(),
                _ => throw new ArgumentException("Неподдерживаемая степень")
            };
            return generator.Generate(p);
        }

        private Result SolveEquation(Equation eq)
        {
            Solver solver = eq.Degree switch
            {
                1 => new LinearSolver(),
                2 => new QuadSolver(),
                3 => new CubicSolver(),
                4 => new QuarticSolver(),
                _ => throw new ArgumentException("Неподдерживаемая степень")
            };
            return solver.Solve(eq);
        }
    }
}