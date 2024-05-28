using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public record OrderCreated : DomainEvent
{
    public Guid OrderId { get; init; }
}

public record OrderAssignedToCourier : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid CourierId { get; init; }
}

public record OrderCompleted : DomainEvent
{
    public Guid OrderId { get; init; }
    public Guid CourierId { get; init; }
}