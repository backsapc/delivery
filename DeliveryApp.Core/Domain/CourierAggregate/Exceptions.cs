using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;

namespace DeliveryApp.Core.Domain.CourierAggregate;

public static class Exceptions
{
    public static DomainException CourierIsAlreadyBusyToGetOnTheLine(Guid courierId) 
        => new($"Courier '{courierId}' is already busy to get on the line");
    
    public static DomainException CourierIsAlreadyBusyToGetOffTheLine(Guid courierId) 
        => new($"Courier '{courierId}' is already busy to get off the line");
    
    public static DomainException CourierIsAlreadyBusyToAcceptTheOrder(Guid courierId) 
        => new($"Courier '{courierId}' is already busy to accept the order");
    
    public static DomainException CourierIsNotAvailableToAcceptTheOrder(Guid courierId) 
        => new($"Courier '{courierId}' is not available to accept the order");
}