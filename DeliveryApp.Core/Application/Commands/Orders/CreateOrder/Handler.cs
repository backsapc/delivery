using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.Orders.CreateOrder;

public class Handler : IRequestHandler<Command, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;

    public Handler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
    }
    
    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        // Получаем геопозицию из Geo (пока ставим фэйковое значение)
        var locationCreateRandomResult = RandomLocation();
        if (locationCreateRandomResult.IsFailure) return false;
        var location = locationCreateRandomResult.Value;

        // Создаем вес
        var weight = Weight.Of(request.Weight);

        // Создаем заказ
        var order = Order.Create(request.BasketId, location, weight);

        // Сохраняем
        _orderRepository.Add(order);
        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }

    private static Result<Location, ValidationException> RandomLocation()
    {
        return Location.Of(new Random().Next(1, 11), new Random().Next(1, 11));
    }
}