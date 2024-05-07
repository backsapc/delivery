using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;

namespace DeliveryApp.Core.DomainServices.Dispatch;

public interface IDispatchService
{
    void Dispatch(Order order, IReadOnlyList<Courier> couriers);
}