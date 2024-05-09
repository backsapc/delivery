using MediatR;

namespace DeliveryApp.Core.Application.Queries.Orders.GetActiveOrders;

public class Query : IRequest<Response>
{
    
}

public class Response
{
    public IReadOnlyCollection<Order> Orders { get; private init; }

    public Response(IReadOnlyCollection<Order> orders)
    {
        Orders = orders;
    }
}

public class Order
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Геопозиция (X,Y)
    /// </summary>
    public Location Location { get; set; }
}

public class Location
{
    /// <summary>
    /// Горизонталь
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Вертикаль
    /// </summary>
    public int Y { get; set; }
}