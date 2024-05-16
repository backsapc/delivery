using MediatR;

namespace DeliveryApp.Core.Application.Commands.Couriers.MoveCouriers;

public class Command : IRequest<bool>;