using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate;

public class OrderAcceptedByCourier : IDomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CourierId { get; set; }
}

public class CourierDeliveredOrder : IDomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CourierId { get; set; }
}