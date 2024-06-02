namespace AnimalLibrary;

public class Dog : Animal
{
    private double Speed { get; set; }

    public Dog(double speed)
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
        Speed = GetSpeed() + 10;

        return ToString();
    }

    public override string Stand()
    {
        Speed = GetSpeed() - 10;

        if (Speed < 0) { Speed = 0; }
        return GetSpeed() <= 0 ? "Собака стоит" : ToString();
    }

    public string MakeVoice()
    {
        return "Гав";
    }
    
    public override string ToString()
    {
        return $"Скорость собаки - {Speed}.";
    }
}