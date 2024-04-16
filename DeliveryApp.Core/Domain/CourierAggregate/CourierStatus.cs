using DeliveryApp.Core.Domain.SharedKernel;

namespace DeliveryApp.Core.Domain.CourierAggregate;

public sealed class CourierStatus : ValueObject
{
    public static CourierStatus NotAvailable = new(1);
    public static CourierStatus Ready = new(2);
    public static CourierStatus Busy = new(3);

    private readonly int _value;

    public CourierStatus(int value)
    {
        _value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}