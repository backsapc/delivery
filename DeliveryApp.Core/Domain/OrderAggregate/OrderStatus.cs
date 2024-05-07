using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public sealed class OrderStatus : ValueObject
{
    public static OrderStatus Created = new(1);
    public static OrderStatus Assigned = new(2);
    public static OrderStatus Completed = new(3);

    public int Value { get; init; }

    private OrderStatus()
    { }
    
    private OrderStatus(int value)
    {
        Value = value;
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}