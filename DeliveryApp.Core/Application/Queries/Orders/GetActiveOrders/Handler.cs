using System.Data;
using Dapper;
using DeliveryApp.Core.Domain.OrderAggregate;
using MediatR;

namespace DeliveryApp.Core.Application.Queries.Orders.GetActiveOrders;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly GetDbConnection _getDbConnection;

    public Handler(GetDbConnection getDbConnection)
    {
        _getDbConnection = getDbConnection;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = _getDbConnection();
        connection.Open();

        const string sql = @"
select id, courier_id, location_x, location_y, weight, status 
from public.orders where status != @status;";

        var result = await connection.QueryAsync<dynamic>(
            sql, new { status = OrderStatus.Completed.Value });

        var items = result.AsList();

        if (items.Count == 0)
            return new Response(Array.Empty<Order>());

        var orders = items.Select(item => MapToOrder(item))
                          .Select(x => (Order)x)
                          .ToList();

        return new Response(orders);
    }

    private static Order MapToOrder(dynamic o) => new()
    {
        Id = o.id,
        Location = new Location
        {
            X = o.location_x,
            Y = o.location_y,
        }
    };
}