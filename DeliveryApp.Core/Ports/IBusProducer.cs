using DeliveryApp.Core.Domain.OrderAggregate;

namespace DeliveryApp.Core.Ports;

public interface IBusProducer
{
    Task Publish(OrderCreated notification, CancellationToken cancellationToken);
    Task Publish(OrderAssignedToCourier notification, CancellationToken cancellationToken);
    Task Publish(OrderCompleted notification, CancellationToken cancellationToken);
}