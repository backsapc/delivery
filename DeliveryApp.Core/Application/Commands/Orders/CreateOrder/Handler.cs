using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.Orders.CreateOrder;

public class Handler : IRequestHandler<Command, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IGeoClient _geoClient;

    public Handler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IGeoClient geoClient)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _geoClient = geoClient;
    }
    
    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        // Получаем геопозицию из Geo
        var location = await _geoClient.Location(request.Address, cancellationToken);
        if (location.IsFailure) return false;
        
        // Создаем вес
        var weight = Weight.Of(request.Weight);

        // Создаем заказ
        var order = Order.Create(request.BasketId, location.Value, weight);

        // Сохраняем
        _orderRepository.Add(order);
        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}