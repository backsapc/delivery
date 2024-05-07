using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.CourierAggregate;

public sealed class CourierStatus : ValueObject
{
    public static CourierStatus NotAvailable = new(1);
    public static CourierStatus Ready = new(2);
    public static CourierStatus Busy = new(3);

    public int Value { get; init; }

    private CourierStatus()
    { }
    
    private CourierStatus(int value)
    {
        Value = value;
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}