using System.Collections.Generic;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;
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
        var value = Location.Of(x, y);

        value.IsSuccess.Should().BeTrue();
        value.Value.PositionX.Should().Be(x);
        value.Value.PositionY.Should().Be(y);
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