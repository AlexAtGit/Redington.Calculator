namespace Redington.Calculation.Infrastructure.Tests.Functions;

using Redington.Calculator.Infrastructure.Functions;

using FluentAssertions;
using Xunit;

public class EitherTests
{
    private readonly EitherFunction _function;

    public EitherTests()
    {
        _function = new EitherFunction();
    }

    [Fact]
    public void Name_ShouldReturnEither()
    {
        // Act
        var name = _function.Name;

        // Assert
        name.Should().Be("Either");
    }

    [Theory]
    [InlineData(0.5, 0.5, 0.75)]
    [InlineData(0.5, 0.4, 0.70)]
    [InlineData(0.1, 0.3, 0.37)]
    public void Execute_ShouldEitherTwoProbabilities(double probabilityA, double probabilityB, double expected)
    {
        // Act
        var result = _function.Execute(probabilityA, probabilityB);

        // Assert
        result.Should().Be(expected);
    }
}
