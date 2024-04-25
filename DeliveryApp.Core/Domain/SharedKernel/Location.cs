using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;

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

    public static Result<Location, ValidationException> Of(int x, int y)
    {
        if (x is not (>= 1 and <= 10)) return Exceptions.ShouldBeInRangeException(x, new Range(1, 10));
        if (y is not (>= 1 and <= 10)) return Exceptions.ShouldBeInRangeException(y, new Range(1, 10));

        return new Location(x, y);
    }

    public long DistanceTo(Location other)
    {
        return Math.Abs(PositionX - other.PositionX) + Math.Abs(PositionY - other.PositionY);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return PositionX;
        yield return PositionY;
    }
    
    private static class Exceptions
    {
        public static ValidationException ShouldBeInRangeException(int value, Range range)
        {
            return new ValidationException(
                nameof(value), $"Should be in range from {range.Start} to {range.End}");
        }
    }
}