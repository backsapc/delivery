using MediatR;

namespace DeliveryApp.Core.Application.Commands.Couriers.GetOffTheLine;

public class Command : IRequest<bool>
{
    public Guid CourierId { get; set; }
}