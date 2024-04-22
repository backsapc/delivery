using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public class OrderAssignedToCourier : IDomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CourierId { get; set; }
}