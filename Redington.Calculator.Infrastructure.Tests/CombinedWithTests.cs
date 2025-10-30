namespace Redington.Calculation.Infrastructure.Tests;

using Redington.Calculator.Infrastructure.Functions;

using FluentAssertions;
using Xunit;

public class CombinedWithTests
{
    private readonly CombinedWithFunction _function;

    public CombinedWithTests()
    {
        _function = new CombinedWithFunction();
    }

    [Fact]
    public void Name_ShouldReturnCombinedWith()
    {
        // Act
        var name = _function.Name;

        // Assert
        name.Should().Be("CombinedWith");
    }

    [Theory]
    [InlineData(0.5, 0.5, 0.25)]
    [InlineData(0.5, 0.4, 0.20)]
    [InlineData(0.1, 0.3, 0.03)]
    public void Execute_ShouldCombineTwoProbabilities(double probabilityA, double probabilityB, double expected)
    {
        // Act
        var result = _function.Execute(probabilityA, probabilityB);

        // Assert
        result.Should().Be(expected);
    }
}
