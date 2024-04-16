namespace DeliveryApp.Core.Domain.SharedKernel;

public sealed class Location : ValueObject
{
    public int PositionY { get; }
    public int PositionX { get; }
    
    private Location(int x, int y)
    {
        PositionX = x;
        PositionY = y;  
    }

    public static Location Of(int x, int y)
    {
        EnsureLessOrEqualThan10(x);
        EnsureGreaterOrEqualThan1(x);
        EnsureLessOrEqualThan10(y);
        EnsureGreaterOrEqualThan1(y);

        return new Location(x, y);
    }

    static void EnsureLessOrEqualThan10(int value)
    {
        if (value > 10)
        {
            throw new ValidationException(nameof(value), "Should be less or equal than 10");
        }
    }

    static void EnsureGreaterOrEqualThan1(int value)
    {
        if (value < 1)
        {
            throw new ValidationException(nameof(value), "Should be greater or equal than 1");
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PositionX;
        yield return PositionY;
    }
}