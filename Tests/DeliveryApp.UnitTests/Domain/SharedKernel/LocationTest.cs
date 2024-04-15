using System.Collections.Generic;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

[TestSubject(typeof(Location))]
public class LocationTest
{

    [Theory]
    [MemberData(nameof(CorrectLocations))]
    public void Location_ShouldBe_CreatedForCorrectData(int x, int y)
    {
        Location value = Location.Of(x, y);

        value.Should().NotBeNull();
        value.PositionX.Should().Be(x);
        value.PositionY.Should().Be(y);
    }

    public static IEnumerable<object[]> CorrectLocations => new object[][]
    {
        [1, 1],
        [10, 10],
        [1, 10],
        [10, 1],
        [5, 5]
    };
    
    [Theory]
    [MemberData(nameof(IncorrectLocations))]
    public void Location_Should_ThrowForIncorrectData(int x, int y)
    {
        var func = () => Location.Of(x, y);

        func.Should().Throw<ValidationException>();
    }
    
    public static IEnumerable<object[]> IncorrectLocations => new object[][]
    {
        [0, 1],
        [11, 10],
        [0, 0],
        [11, 11]
    };
}