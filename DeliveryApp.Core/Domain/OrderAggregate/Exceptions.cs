using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public static class Exceptions
{
    public static DomainException CompletedOrderCannotBeAssigned(Guid orderId)
        => new DomainException($"Cannot assign completed order '{orderId}' to courier");
}