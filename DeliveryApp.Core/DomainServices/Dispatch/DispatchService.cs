using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;

namespace DeliveryApp.Core.DomainServices.Dispatch;

public class DispatchService : IDispatchService
{
    public Result<Courier, DomainException> Dispatch(Order order, IReadOnlyCollection<Courier> couriers)
    {
        var bestCourier = couriers
                          .Where(x => x.CanHandle(order.Weight))
                          .MinBy(x => x.CalculateTime(order.DeliveryLocation));

        if (bestCourier == null)
            return Exceptions.NoCouriersFound(order.Id);

        bestCourier.AcceptOrder(order.Id);
        order.AssignTo(bestCourier.Id);

        return bestCourier;
    }
}