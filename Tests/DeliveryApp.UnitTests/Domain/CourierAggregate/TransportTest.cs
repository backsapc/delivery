using System.Collections.Generic;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.CourierAggregate;

[TestSubject(typeof(Transport))]
public class TransportTest
{
    [Theory]
    [MemberData(nameof(CorrectData))]
    public void Should_Calculate_Steps_Correctly(Transport transport, int x1, int y1, int x2, int y2, long expectedSteps)
    {
        var location1 = Location.Of(x1, y1).Value;
        var location2 = Location.Of(x2, y2).Value;

        var actualSteps = transport.CalculateTime(location1, location2);

        actualSteps.Should().Be(expectedSteps);
    }

    public static IEnumerable<object[]> CorrectData()
    {
        yield return [Transport.Pedestrian, 1, 1, 2, 2, 2];
        yield return [Transport.Pedestrian, 2, 2, 1, 1, 2];
        yield return [Transport.Pedestrian, 1, 1, 1, 1, 0];
        yield return [Transport.Pedestrian, 1, 1, 10, 10, 18];

        yield return [Transport.Bicycle, 1, 1, 1, 1, 0];
        yield return [Transport.Bicycle, 1, 1, 2, 2, 1];
        yield return [Transport.Bicycle, 1, 1, 2, 3, 2];
        yield return [Transport.Bicycle, 1, 1, 10, 10, 9];

        yield return [Transport.Scooter, 1, 1, 1, 1, 0];
        yield return [Transport.Scooter, 1, 1, 2, 2, 1];
        yield return [Transport.Scooter, 1, 1, 2, 4, 2];
        yield return [Transport.Scooter, 1, 1, 10, 10, 6];

        yield return [Transport.Car, 1, 1, 1, 1, 0];
        yield return [Transport.Car, 1, 1, 2, 2, 1];
        yield return [Transport.Car, 1, 1, 2, 5, 2];
        yield return [Transport.Car, 1, 1, 10, 10, 5];
    }
}