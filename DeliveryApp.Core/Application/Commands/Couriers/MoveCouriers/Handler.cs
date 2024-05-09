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
    private readonly GetAllAssignedOrders _getAllAssignedOrders;

    public Handler(
        IUnitOfWork unitOfWork,
        ICourierRepository courierRepository,
        IOrderRepository orderRepository,
        GetAllAssignedOrders getAllAssignedOrders)
    {
        _unitOfWork = unitOfWork;
        _courierRepository = courierRepository;
        _orderRepository = orderRepository;
        _getAllAssignedOrders = getAllAssignedOrders;
    }

    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var assignedOrderIds = await _getAllAssignedOrders();

        foreach (Guid orderId in assignedOrderIds)
        {
            var order = await _orderRepository.Get(orderId);
            var courier = await _courierRepository.Get(order!.CourierId!.Value);

            courier!.StepTowardTheOrder(order.DeliveryLocation);
            if (courier.GetDomainEvents().OfType<CourierDeliveredOrder>().Any()) // TODO: Move to async handler
                order.Complete(courier.Id);

            _orderRepository.Update(order);
            _courierRepository.Update(courier);
        }
        
        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}