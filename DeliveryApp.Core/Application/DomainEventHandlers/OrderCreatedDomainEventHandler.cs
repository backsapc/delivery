using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers
{
    public sealed class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreated>
    {
        private readonly IBusProducer _busProducer;

        public OrderCreatedDomainEventHandler(IBusProducer busProducer)
        {
            _busProducer = busProducer;
        }

        public async Task Handle(OrderCreated notification, CancellationToken cancellationToken)
        {
            await _busProducer.Publish(notification, cancellationToken);
        }
    }
}