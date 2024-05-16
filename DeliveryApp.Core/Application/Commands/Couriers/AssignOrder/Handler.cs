using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.DomainServices.Dispatch;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.Couriers.AssignOrder;

public class Handler : IRequestHandler<Command, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly ICourierRepository _courierRepository;
    private readonly IDispatchService _dispatchService;

    public Handler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        ICourierRepository courierRepository,
        IDispatchService dispatchService)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _courierRepository = courierRepository;
        _dispatchService = dispatchService;
    }

    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var unassignedOrders = await _orderRepository.UnassignedOrders();
        var order = unassignedOrders.FirstOrDefault();
        if (order == null)
            return false;

        var readyCouriers = await _courierRepository.ReadyCouriers();
        if (readyCouriers.Count == 0)
            return false;

        var dispatchResult = _dispatchService.Dispatch(order, readyCouriers);
        if (dispatchResult.IsFailure) return false;

        var courier = dispatchResult.Value;
        
        _courierRepository.Update(courier); // Change tracking?
        _orderRepository.Update(order);
        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}