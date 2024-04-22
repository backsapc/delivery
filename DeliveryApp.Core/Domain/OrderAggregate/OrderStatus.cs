using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public sealed class OrderStatus : ValueObject
{
    public static OrderStatus Created = new(1);
    public static OrderStatus Assigned = new(2);
    public static OrderStatus Completed = new(3);
    
    private readonly int _value;

    private OrderStatus(int value)
    {
        _value = value;
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return _value;
    }
}