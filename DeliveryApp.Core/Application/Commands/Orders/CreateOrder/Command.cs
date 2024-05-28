using MediatR;

namespace DeliveryApp.Core.Application.Commands.Orders.CreateOrder;

public class Command : IRequest<bool>
{
    /// <summary>
    /// Идентификатор корзины
    /// </summary>
    /// <remarks>Id корзины берется за основу при создании Id заказа, они совпадают</remarks>
    public Guid BasketId { get; init; }

    /// <summary>
    /// Адрес
    /// </summary>
    public string Address { get; init; }

    /// <summary>
    /// Вес
    /// </summary>
    public int Weight { get; init; }
}