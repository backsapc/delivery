using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.Couriers.MoveCouriers;

public class Handler : IRequestHandler<Command, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICourierRepository _courierRepository;
    private readonly IOrderRepository _orderRepository;

    public Handler(
        IUnitOfWork unitOfWork,
        ICourierRepository courierRepository,
        IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork;
        _courierRepository = courierRepository;
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var assignedOrderIds = await _orderRepository.AssignedOrders();
        
        foreach (var order in assignedOrderIds)
        {
            var courier = await _courierRepository.Get(order.CourierId!.Value);

            courier!.StepTowardTheOrder(order.DeliveryLocation);
            if (courier.GetDomainEvents().OfType<CourierDeliveredOrder>().Any()) // TODO: Move to async handler
                order.Complete(courier.Id);

            _orderRepository.Update(order);
            _courierRepository.Update(courier);
        }
        
        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}