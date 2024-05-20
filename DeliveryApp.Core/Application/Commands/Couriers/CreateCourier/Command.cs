using MediatR;

namespace DeliveryApp.Core.Application.Commands.Couriers.CreateCourier;

public class Command : IRequest<bool>
{
    public Guid CourierId { get; init; }
    public string CourierName { get; init; }
    public string Transport { get; init; }
}