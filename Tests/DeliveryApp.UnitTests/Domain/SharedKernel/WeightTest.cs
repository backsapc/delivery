using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.SharedKernel;

[TestSubject(typeof(Weight))]
public class WeightTest
{

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Should_Create_CorrectWeight(int weight)
    {
        var value = Weight.Of(weight);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Should_Throw_ValidationException(int weight)
    {
        var func = () => Weight.Of(weight);

        func.Should().Throw<ValidationException>();
    }
}