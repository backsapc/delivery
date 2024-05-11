using DeliveryApp.Core.Domain.CourierAggregate;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.Couriers.GetOffTheLine;

public class Handler : IRequestHandler<Command, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICourierRepository _courierRepository;

    public Handler(IUnitOfWork unitOfWork,
                   ICourierRepository courierRepository)
    {
        _unitOfWork = unitOfWork;
        _courierRepository = courierRepository;
    }
    
    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var courier = await _courierRepository.Get(request.CourierId);
        
        courier!.GetOffTheLine();

        _courierRepository.Update(courier);
        return await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}