namespace CarTuner
{
    // Base class for all car parts.
    public abstract class CarPart
    {
        public string Name { get; set; } = string.Empty;
        public int SpeedBonus { get; set; }
        public decimal Cost { get; set; }

        public override string ToString()
        {
            return $"{Name} (+{SpeedBonus} speed, {Cost:C})";
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
}