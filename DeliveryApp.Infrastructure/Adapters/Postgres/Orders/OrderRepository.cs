using DeliveryApp.Core.Domain.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Orders;

public class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    public Order Add(Order order)
    {
        dbContext.Orders.Add(order);

        return order;
    }

    public void Update(Order order)
    {
        dbContext.Orders.Update(order);
    }

    public async Task<Order?> GetAsync(Guid orderId)
    {
        return await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
    }
}