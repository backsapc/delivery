using System;
using System.Collections.Generic;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate;

[TestSubject(typeof(Courier))]
public class CourierTest
{
    [Theory]
    [MemberData(nameof(OneStepFromDefaultLocations))]
    public void Courier_Should_StopAtTargetLocation(Transport transport, (int PosX, int PosY) position)
    {
        // arrange
        var courier = Courier.Create(
            Guid.NewGuid(), 
            Guid.NewGuid().ToString(), 
            transport
        );

        var targetLocation = Location.Of(position.PosX, position.PosY).Value;

        // act
        courier.StepTowardTheOrder(targetLocation);
        
        // assert
        courier.CurrentLocation().Should().Be(targetLocation);
    }

    public static IEnumerable<object[]> OneStepFromDefaultLocations()
    {
        yield return [Transport.Pedestrian, (1, 1)];
        yield return [Transport.Pedestrian, (1, 2)];
        yield return [Transport.Pedestrian, (2, 1)];

        yield return [Transport.Bicycle, (1, 1)];
        yield return [Transport.Bicycle, (1, 2)];
        yield return [Transport.Bicycle, (2, 2)];

        yield return [Transport.Scooter, (1, 1)];
        yield return [Transport.Scooter, (1, 2)];
        yield return [Transport.Scooter, (2, 3)];

        yield return [Transport.Car, (1, 1)];
        yield return [Transport.Car, (1, 2)];
        yield return [Transport.Car, (3, 3)];
    }
    
    [Theory]
    [MemberData(nameof(MoreThanOneStepFromDefaultLocation))]
    public void Courier_Should_BeMovingTowardTheLocation(Transport transport, (int PosX, int PosY) position)
    {
        // arrange
        var courier = Courier.Create(
            Guid.NewGuid(), 
            Guid.NewGuid().ToString(), 
            transport
        );

        var initialLocation = courier.CurrentLocation();
        var targetLocation = Location.Of(position.PosX, position.PosY).Value;
        var initialDistance = initialLocation.DistanceTo(targetLocation);

        // act
        courier.StepTowardTheOrder(targetLocation);
        
        // assert
        var actualLocation = courier.CurrentLocation();
        actualLocation.Should().NotBe(targetLocation);

        var actualDistance = actualLocation.DistanceTo(targetLocation);
        actualDistance.Should().BeLessThan(initialDistance);
    }

    public static IEnumerable<object[]> MoreThanOneStepFromDefaultLocation()
    {
        yield return [Transport.Pedestrian, (1, 3)];
        yield return [Transport.Pedestrian, (3, 1)];

        yield return [Transport.Bicycle, (1, 4)];
        yield return [Transport.Bicycle, (4, 1)];

        yield return [Transport.Scooter, (5, 1)];
        yield return [Transport.Scooter, (1, 5)];

        yield return [Transport.Car, (6, 1)];
        yield return [Transport.Car, (1, 6)];
    }
    
    [Fact]
    public void Courier_Should_NotAcceptOrder_Because_IsNotOnTheLine()
    {
        // arrange
        var courier = Courier.Create(
            Guid.NewGuid(), 
            Guid.NewGuid().ToString(), 
            Transport.Pedestrian
        );

        var orderId = Guid.NewGuid();

        // act
        var action = () => courier.AcceptOrder(orderId);
        
        // assert
        action.Should().Throw<DomainException>();
    }
    
    [Fact]
    public void Courier_Should_NotAcceptOrder_Because_IsBusy()
    {
        // arrange
        var courier = Courier.Create(
            Guid.NewGuid(), 
            Guid.NewGuid().ToString(), 
            Transport.Pedestrian
        );

        var orderId = Guid.NewGuid();
        var anotherOrderId = Guid.NewGuid();
        
        courier.GetOnTheLine();
        courier.AcceptOrder(orderId);

        // act
        var action = () => courier.AcceptOrder(anotherOrderId);
        
        // assert
        action.Should().Throw<DomainException>();
    }
    
    [Fact]
    public void Courier_Should_AcceptOrder()
    {
        // arrange
        var courier = Courier.Create(
            Guid.NewGuid(), 
            Guid.NewGuid().ToString(), 
            Transport.Pedestrian
        );
        
        courier.GetOnTheLine();

        var orderId = Guid.NewGuid();
        
        var expectedEvent = new OrderAcceptedByCourier
        {
            OrderId = orderId,
            CourierId = courier.Id
        };

        // act
        courier.AcceptOrder(orderId);
        
        // assert
        var courierStatus = courier.CurrentStatus();
        courierStatus.Should().Be(CourierStatus.Busy);
        
        var emittedEvents = courier.GetDomainEvents();
        emittedEvents.Should().NotBeEmpty();
        emittedEvents.Should().ContainEquivalentOf(expectedEvent);
    }
    
    [Fact]
    public void Courier_Should_CompleteOrder()
    {
        // arrange
        var courier = Courier.Create(
            Guid.NewGuid(), 
            Guid.NewGuid().ToString(), 
            Transport.Pedestrian
        );

        courier.GetOnTheLine();
        
        var orderId = Guid.NewGuid();
        var targetLocation = Location.Of(1, 2).Value;
        
        courier.AcceptOrder(orderId);

        var expectedEvent = new CourierDeliveredOrder
        {
            OrderId = orderId,
            CourierId = courier.Id
        };

        // act
        courier.StepTowardTheOrder(targetLocation);
        
        // assert
        var actualLocation = courier.CurrentLocation();
        actualLocation.Should().Be(targetLocation);

        var emittedEvents = courier.GetDomainEvents();
        emittedEvents.Should().NotBeEmpty();
        emittedEvents.Should().ContainEquivalentOf(expectedEvent);
    }
}