namespace CarTuner
{
    public abstract class CarPart
    {
        public string Name { get; set; }
        public int SpeedBonus { get; set; }
        public decimal Cost { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }

        public override string ToString()
        {
            return Name + " (+" + SpeedBonus + " speed, " + Cost.ToString("C") + ")";
        }
    }

    public class Engine : CarPart
    {
        public int CylinderCount { get; set; }
    }

    public class Exhaust : CarPart
    {
        public bool IsSport { get; set; }
    }

    public class Wheel : CarPart
    {
        public int SizeInInches { get; set; }
    }

    public class Turbo : CarPart
    {
        public string BoostType { get; set; }
    }

    public class Intake : CarPart
    {
        public string IntakeType { get; set; }
    }

    public class ECUTune : CarPart
    {
        public int Stage { get; set; }
    }

    public class Intercooler : CarPart
    {
        public string CoolerType { get; set; }
    }

    public class Transmission : CarPart
    {
        public string TransType { get; set; }
    }

    public class Brake : CarPart
    {
        public int PistonCount { get; set; }
    }
}