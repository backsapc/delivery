using DeliveryApp.Core.Domain.CourierAggregate;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.Couriers.CreateCourier;

public class Handler : IRequestHandler<Command, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICourierRepository _courierRepository;

    public Handler(
        IUnitOfWork unitOfWork,
        ICourierRepository courierRepository)
    {
        _unitOfWork = unitOfWork;
        _courierRepository = courierRepository;
    }
    
    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var transport = Transport.FromName(request.Transport);
        if (transport.IsFailure) return false;
        
        var courier = Courier.Create(request.CourierId, request.CourierName, transport.Value);

        // Сохраняем
        _courierRepository.Add(courier);
        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}