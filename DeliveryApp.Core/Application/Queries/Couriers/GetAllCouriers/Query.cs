using MediatR;

namespace DeliveryApp.Core.Application.Queries.Couriers.GetAllCouriers;

public class Query : IRequest<Response>;

public class Response
{
    public IReadOnlyCollection<Courier> Couriers { get; }

    public Response(IReadOnlyCollection<Courier> couriers)
    {
        Couriers = couriers;
    }
}

public class Courier
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; set; }

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