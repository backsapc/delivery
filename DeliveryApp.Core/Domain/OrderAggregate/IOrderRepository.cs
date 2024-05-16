using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public interface IOrderRepository : IRepository<Order>
{
    Order Add(Order order);
    void Update(Order order);
    Task<Order?> Get(Guid orderId);
    Task<IReadOnlyCollection<Order>> UnassignedOrders();
    Task<IReadOnlyCollection<Order>> AssignedOrders();
}