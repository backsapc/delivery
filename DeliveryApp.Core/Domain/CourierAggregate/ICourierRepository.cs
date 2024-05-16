using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate;

public interface ICourierRepository : IRepository<Courier>
{
    Courier Add(Courier courier);
    void Update(Courier courier);
    Task<Courier?> Get(Guid courierId);
    Task<IReadOnlyCollection<Courier>> ReadyCouriers();
}