namespace AnimalLibrary;

public class Panther : Animal
{
    private double Speed { get; set; }

    public Panther(double speed)
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
        Speed = GetSpeed() + 20;

        return ToString();
    }

    public override string Stand()
    {
        Speed = GetSpeed() - 20;
        if (Speed< 0) { Speed = 0; }
        return GetSpeed() <= 0 ? "Пантера стоит" : ToString();
    }

    public string MakeVoice()
    {
        return "Мяу";
    }

    public string ClimbOnTree()
    {
        return "Пантера на дереве";
    }

    public override string ToString()
    {
        return $"Скорость пантеры - {Speed}.";
    }
}