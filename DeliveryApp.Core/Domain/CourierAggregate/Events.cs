using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate;

public record OrderAcceptedByCourier : DomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CourierId { get; set; }
}

public record CourierDeliveredOrder : DomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CourierId { get; set; }
}