using MediatR;

namespace DeliveryApp.Core.Application.Commands.Couriers.AssignOrder;

public class Command : IRequest<bool>;