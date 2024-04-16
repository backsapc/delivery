using System.Collections.Generic;
using DeliveryApp.Core.Domain.CourierAggregate;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate;

[TestSubject(typeof(CourierStatus))]
public class CourierStatusTest
{

    [Fact]
    public void Should_Create_CourierStatus_NotAvailable()
    {
        var status = CourierStatus.NotAvailable;
        
        status.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Create_CourierStatus_Busy()
    {
        var status = CourierStatus.Busy;

        status.Should().NotBeNull();
    }
    
    [Fact]
    public void Should_Create_CourierStatus_Ready()
    {
        var status = CourierStatus.Ready;
        
        status.Should().NotBeNull();
    }
    
    [Theory]
    [MemberData(nameof(EqualsTypes))]
    public void CreatedValues_ShouldBe_Equals(CourierStatus value1, CourierStatus value2 )
    {
        var equalsResult = value1.Equals(value2);
        var operatorEqualsResult = value1 == value2;

        equalsResult.Should().BeTrue();
        operatorEqualsResult.Should().BeTrue();
    }
    
    [Theory]
    [MemberData(nameof(NotEqualsTypes))]
    public void CreatedValues_ShouldBe_NotEquals(CourierStatus value1, CourierStatus value2 )
    {
        var equalsResult = value1.Equals(value2);
        var operatorEqualsResult = value1 == value2;

        equalsResult.Should().BeFalse();
        operatorEqualsResult.Should().BeFalse();
    }
    
    public static IEnumerable<object[]> EqualsTypes =>
        new List<object[]>
        {
            new object[] { CourierStatus.NotAvailable, CourierStatus.NotAvailable },
            new object[] { CourierStatus.Busy, CourierStatus.Busy },
            new object[] { CourierStatus.Ready, CourierStatus.Ready },
        };
    
    public static IEnumerable<object[]> NotEqualsTypes =>
        new List<object[]>
        {
            new object[] { CourierStatus.NotAvailable, CourierStatus.Busy },
            new object[] { CourierStatus.Busy, CourierStatus.Ready },
            new object[] { CourierStatus.Ready, CourierStatus.NotAvailable },
        };
}