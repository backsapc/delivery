using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public static class Exceptions
{
    public static DomainException CompletedOrderCannotBeAssigned(Guid orderId)
        => new($"Cannot assign completed order '{orderId}' to courier");
    
    public static DomainException CannotCompleteOrderByWrongCourier(Guid orderId, Guid courierId)
        => new($"Cannot complete order '{orderId}' to by '{courierId}'courier");
    
    public static DomainException CannotCompleteNotInProgressOrder(Guid orderId)
        => new($"Cannot complete not in progress order '{orderId}'");
}