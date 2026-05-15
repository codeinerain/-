public class StepByStepSession
{
    public int CurrentStep { get; set; }
    public int Degree { get; set; }

    // Общие коэффициенты
    public double A { get; set; }
    public double B { get; set; }
    public double C { get; set; }
    public double D { get; set; }
    public double E { get; set; }  // для 4 степени

    // Для квадратных
    public double P { get; set; }
    public double Q { get; set; }
    public double Shift { get; set; }
    public double Delta { get; set; }

    // Для кубических
    public double Alpha { get; set; }
    public double Beta { get; set; }

    // Для 4 степени (Феррари)
    public double P1 { get; set; }  // p после замены
    public double Q1 { get; set; }  // q после замены
    public double R1 { get; set; }  // r после замены
    public double T0 { get; set; }  // корень резольвенты
    public double A1 { get; set; }  // A = √t
    public double B1 { get; set; }  // B = q/(2A)
    public double C1 { get; set; }  // C = (p+t)/2

    // Корни
    public double? Root1 { get; set; }
    public double? Root2 { get; set; }
    public double? Root3 { get; set; }
    public double? Root4 { get; set; }
    public double Root2Real { get; set; }
    public double Root2Imag { get; set; }
    public double Root3Real { get; set; }
    public double Root3Imag { get; set; }
    public double Root4Real { get; set; }
    public double Root4Imag { get; set; }

    public string Equation { get; set; }
    public bool IsCompleted { get; set; }
}