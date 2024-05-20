using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public sealed class Order : Aggregate<Guid>
{
    public Guid? CourierId { get; private set; }
    public Location DeliveryLocation { get; private set; }
    public Weight Weight { get; private set; }
    public OrderStatus Status { get; private set; }

    private Order() { }
    
    private Order(Guid orderId, Location deliveryLocation, Weight weight, OrderStatus status) : base(orderId)
    {
        DeliveryLocation = deliveryLocation;
        Weight = weight;
        Status = status;
    }

    public static Order Create(Guid orderId, Location location, Weight weight)
    {
        var order = new Order(
            orderId,
            Ensure.NotNull(location),
            Ensure.NotNull(weight),
            OrderStatus.Created);
        
        // Место плохое, но лучше места всё равно нет
        order.RaiseDomainEvent(new OrderCreated { OrderId = orderId }); 

        return order;
    }

    public void AssignTo(Guid courierId)
    {
        if (Status == OrderStatus.Completed) 
            throw Exceptions.CompletedOrderCannotBeAssigned(Id);
        
        CourierId = courierId;
        Status = OrderStatus.Assigned;
        
        RaiseDomainEvent(new OrderAssignedToCourier
        {
            OrderId = Id, 
            CourierId = courierId
        });
    }
    
    public void Complete(Guid courierId)
    {
        if (Status != OrderStatus.Assigned) 
            throw Exceptions.CannotCompleteNotInProgressOrder(Id);

        if (CourierId != courierId)
            throw Exceptions.CannotCompleteOrderByWrongCourier(Id, courierId);
        
        CourierId = courierId;
        Status = OrderStatus.Completed;
        
        RaiseDomainEvent(new OrderCompleted
        {
            OrderId = Id, 
            CourierId = courierId
        });
    }
}