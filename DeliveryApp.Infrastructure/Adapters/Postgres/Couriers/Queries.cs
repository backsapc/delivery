using Dapper;
using DeliveryApp.Core.Application.Queries;
using DeliveryApp.Core.Domain.CourierAggregate;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Couriers;

public static class Queries
{
    public static async Task<Guid[]> GetAllReadyCouriers(GetDbConnection getDbConnection)
    {
        using var connection = getDbConnection();
        connection.Open();

        const string sql = @"select id from public.couriers where status == @status";

        var result = await connection.QueryAsync<Guid>(
            sql, new { status = CourierStatus.Ready.Value });

        return result.ToArray();
    }
}