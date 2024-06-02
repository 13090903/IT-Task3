namespace AnimalLibrary;

public class Turtle : Animal
{
    private double Speed { get; set; }

    public Turtle(double speed)
    {
        if (speed < 0)
        {
            throw new ArgumentException("Скорость не может быть отрицательной");
        }

        Speed = speed;
    }

    public override double GetSpeed()
    {
        return Speed;
    }

    public override string Move()
    {
        Speed = GetSpeed() + 1;

        return ToString();
    }

    public override string Stand()
    {
        Speed = GetSpeed() - 1;

        if (Speed < 0) { Speed = 0; }
        return GetSpeed() <= 0 ? "Черепаха стоит" : ToString();
    }

    public string MakeVoice()
    {
        return "...";
    }
    
    public override string ToString()
    {
        return $"Скорость черепахи - {Speed}.";
    }
}