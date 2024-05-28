using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers
{
    public sealed class OrderCompletedDomainEventHandler : INotificationHandler<OrderCompleted>
    {
        private readonly IBusProducer _busProducer;

        public OrderCompletedDomainEventHandler(IBusProducer busProducer)
        {
            _busProducer = busProducer;
        }

        public async Task Handle(OrderCompleted notification, CancellationToken cancellationToken)
        {
            await _busProducer.Publish(notification, cancellationToken);
        }
    }
}