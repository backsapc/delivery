using DeliveryApp.Core.Domain.SharedKernel.Exceptions;

namespace DeliveryApp.Core.DomainServices.Dispatch;

public static class Exceptions
{
    public static DomainException NoCouriersFound(Guid orderId) => 
        new($"No couriers can handle the order \"{orderId}\" at the time.");
}