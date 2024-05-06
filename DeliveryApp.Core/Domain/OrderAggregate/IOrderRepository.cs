using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public interface IOrderRepository : IRepository<Order>
{
    Order Add(Order order);              // Добавить заказ
    void Update(Order order);            // Обновить заказ
    Task<Order?> Get(Guid orderId); //Получить заказ
}