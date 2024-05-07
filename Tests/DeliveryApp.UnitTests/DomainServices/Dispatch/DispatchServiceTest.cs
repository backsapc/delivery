using System;
using System.Linq;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;
using DeliveryApp.Core.DomainServices.Dispatch;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;
using Exceptions = DeliveryApp.Core.DomainServices.Dispatch.Exceptions;

namespace DeliveryApp.UnitTests.DomainServices.Dispatch;

[TestSubject(typeof(DispatchService))]
public class DispatchServiceTest
{
    [Fact]
    public void Should_AssignCourierWithMinTime()
    {
        // arrange
        var validCourier = Courier.Create(
            Guid.NewGuid(), Guid.NewGuid().ToString(), Transport.Car);
        validCourier.GetOnTheLine();
        
        var invalidCourierWhichNotHandles = Courier.Create(
            Guid.NewGuid(), Guid.NewGuid().ToString(), Transport.Car);
        invalidCourierWhichNotHandles.GetOnTheLine();

        var invalidCourierWhichCanHandle = Courier.Create(
            Guid.NewGuid(), Guid.NewGuid().ToString(), Transport.Scooter);
        invalidCourierWhichCanHandle.GetOnTheLine();

        var couriers = new[]
        {
            validCourier, 
            invalidCourierWhichNotHandles, 
            invalidCourierWhichCanHandle
        };
        
        var order = Order.Create(
            Guid.NewGuid(), Location.Of(10, 10).Value, Weight.Of(6));
        
        var dispatchService = new DispatchService();
        
        // act
        dispatchService.Dispatch(order, couriers);
        
        // assert
        var events = validCourier.GetDomainEvents();
        events.Should().NotBeEmpty();

        var courierAssignedEvent = events.OfType<OrderAcceptedByCourier>().FirstOrDefault();
        courierAssignedEvent.Should().NotBeNull();
        courierAssignedEvent!.OrderId.Should().Be(order.Id);

        invalidCourierWhichNotHandles.GetDomainEvents().Should().BeEmpty();
        invalidCourierWhichCanHandle.GetDomainEvents().Should().BeEmpty();
    }
    
    [Fact]
    public void Should_Throw_IfNoCouriersFound()
    {
        // arrange
        var invalidCourier1 = Courier.Create(
            Guid.NewGuid(), Guid.NewGuid().ToString(), Transport.Bicycle);
        invalidCourier1.GetOnTheLine();
        
        var invalidCourier2 = Courier.Create(
            Guid.NewGuid(), Guid.NewGuid().ToString(), Transport.Pedestrian);
        invalidCourier2.GetOnTheLine();

        var couriers = new[]
        {
            invalidCourier1, 
            invalidCourier2
        };
        
        var order = Order.Create(
            Guid.NewGuid(), Location.Of(10, 10).Value, Weight.Of(6));
        
        var dispatchService = new DispatchService();

        var expectedException = Exceptions.NoCouriersFound(order.Id);
        
        // act
        var action = () => dispatchService.Dispatch(order, couriers);
        
        // assert
        var exception = action.Should().Throw<DomainException>();
        exception.Which.Message.Should().Be(expectedException.Message);
    }
}