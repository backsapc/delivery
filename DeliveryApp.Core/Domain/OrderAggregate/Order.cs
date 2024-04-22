﻿using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.OrderAggregate;

public class Order : Aggregate<Guid>
{
    private Guid? CourierId { get; set; }
    private Location DeliveryLocation { get; set; }
    private Weight Weight { get; set; }
    private OrderStatus Status { get; set; }

    private Order(Guid orderId, Location deliveryLocation, Weight weight, OrderStatus status) : base(orderId)
    {
        DeliveryLocation = deliveryLocation;
        Weight = weight;
        Status = status;
    }

    public static Order Create(Guid orderId, Location location, Weight weight)
    {
        return new Order(
            orderId,
            Ensure.NotNull(location),
            Ensure.NotNull(weight),
            OrderStatus.Created);
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
}