using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;

namespace DeliveryApp.Core.DomainServices.Dispatch;

public interface IDispatchService
{
    Result<Courier, DomainException> Dispatch(Order order, IReadOnlyCollection<Courier> couriers);
}