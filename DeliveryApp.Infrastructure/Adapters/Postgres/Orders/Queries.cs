using Dapper;
using DeliveryApp.Core.Application.Queries;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Orders;

public static class Queries
{
    public static async Task<Guid[]> GetAllAssignedOrders(GetDbConnection getDbConnection)
    {
        using var connection = getDbConnection();
        connection.Open();

        const string sql = @"select id from public.orders where status == @status";

        var result = await connection.QueryAsync<Guid>(
            sql, new { status = OrderStatus.Assigned.Value });

        return result.ToArray();
    }
    
    public static async Task<Guid[]> GetAllNotAssignedOrders(GetDbConnection getDbConnection)
    {
        using var connection = getDbConnection();
        connection.Open();

        const string sql = @"select id from public.orders where status == @status";

        var result = await connection.QueryAsync<Guid>(
            sql, new { status = OrderStatus.Created.Value });

        return result.ToArray();
    }
}