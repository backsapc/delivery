using Dapper;
using DeliveryApp.Core.Domain.CourierAggregate;
using MediatR;

namespace DeliveryApp.Core.Application.Queries.Couriers.GetAllCouriers;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly GetDbConnection _getDbConnection;

    public Handler(GetDbConnection getDbConnection)
    {
        _getDbConnection = getDbConnection;
    }
    
    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        using var connection = _getDbConnection();
        connection.Open();

        const string sql = "select id, name, location_x, location_y from public.couriers";
        var result = await connection.QueryAsync(sql);

        var couriers = result.AsList()
                             .Select(x => Map(x))
                             .Select(x => (Courier)x)
                             .ToArray();
        return new Response(couriers);
    }

    private static Courier Map(dynamic value)
    {
        return new Courier
        {
            Id = value.id,
            Name = value.name,
            Location = new Location
            {
                X = value.location_x,
                Y = value.location_y
            }
        };
    }
}