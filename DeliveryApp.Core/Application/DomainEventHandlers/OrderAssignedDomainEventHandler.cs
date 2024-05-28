using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers
{
    public sealed class OrderAssignedDomainEventHandler : INotificationHandler<OrderAssignedToCourier>
    {
        private readonly IBusProducer _busProducer;

        public OrderAssignedDomainEventHandler(IBusProducer busProducer)
        {
            _busProducer = busProducer;
        }

        public async Task Handle(OrderAssignedToCourier notification, CancellationToken cancellationToken)
        {
            await _busProducer.Publish(notification, cancellationToken);
        }
    }
}