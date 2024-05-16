using MediatR;

namespace DeliveryApp.Core.Application.Commands.Couriers.GetOnTheLine;

public class Command : IRequest<bool>
{
    public Guid CourierId { get; set; }
}