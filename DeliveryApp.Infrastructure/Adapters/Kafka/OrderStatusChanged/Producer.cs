using Confluent.Kafka;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Ports;
using Newtonsoft.Json;
using OrderStatusChanged;
using OrderStatus = OrderStatusChanged.OrderStatus;

namespace DeliveryApp.Infrastructure.Adapters.Kafka.OrderStatusChanged;

public class Producer : IBusProducer
{
    private readonly ProducerConfig _config;
    private readonly string _topicName = "order.status.changed";

    public Producer(string messageBrokerHost)
    {
        if (string.IsNullOrWhiteSpace(messageBrokerHost)) throw new ArgumentException(nameof(messageBrokerHost));
        _config = new ProducerConfig
        {
            BootstrapServers = messageBrokerHost
        };
    }

    public async Task Publish(OrderCreated notification, CancellationToken cancellationToken)
    {
        // Перекладываем данные из Domain Event в Integration Event
        var basketConfirmedIntegrationEvent = new OrderStatusChangedIntegrationEvent
        {
            OrderId = notification.OrderId.ToString(),
            OrderStatus = OrderStatus.Created
        };

        await ProduceInner(notification.EventId, cancellationToken, basketConfirmedIntegrationEvent);
    }

    public async Task Publish(OrderAssignedToCourier notification, CancellationToken cancellationToken)
    {
        // Перекладываем данные из Domain Event в Integration Event
        var basketConfirmedIntegrationEvent = new OrderStatusChangedIntegrationEvent
        {
            OrderId = notification.OrderId.ToString(),
            OrderStatus = OrderStatus.Assigned
        };

        await ProduceInner(notification.EventId, cancellationToken, basketConfirmedIntegrationEvent);
    }

    public async Task Publish(OrderCompleted notification, CancellationToken cancellationToken)
    {
        // Перекладываем данные из Domain Event в Integration Event
        var basketConfirmedIntegrationEvent = new OrderStatusChangedIntegrationEvent
        {
            OrderId = notification.OrderId.ToString(),
            OrderStatus = OrderStatus.Completed
        };

        await ProduceInner(notification.EventId, cancellationToken, basketConfirmedIntegrationEvent);
    }
    
    private async Task ProduceInner(Guid eventId,
                                    CancellationToken cancellationToken,
                                    OrderStatusChangedIntegrationEvent basketConfirmedIntegrationEvent)
    {
        // Создаем сообщение для Kafka
        var message = new Message<string, string>
        {
            Key = eventId.ToString(),
            Value = JsonConvert.SerializeObject(basketConfirmedIntegrationEvent)
        };
        
        // Отправляем сообщение в Kafka
        using var producer = new ProducerBuilder<string, string>(_config).Build();
        await producer.ProduceAsync(_topicName, message, cancellationToken);
    }
}