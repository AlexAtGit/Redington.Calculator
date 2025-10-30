namespace Redington.Calculator.API.Tests.Validation;

using Redington.Calculator.API.Validation;
using Redington.Calculator.Domain.Models;

using Microsoft.Extensions.Options;
using FluentAssertions;
using Xunit;

public class CalculationRequestValidatorTests
{
    private readonly CalculationRequestValidator _validator;

    public CalculationRequestValidatorTests()
    {
        var constants = new Constants { MinProbabilityValue = 0.0, MaxProbabilityValue = 1.0 };
        var options = Options.Create(constants);

        _validator = new CalculationRequestValidator(options);
    }

    [Theory]
    [InlineData(0.2, 0.3, "CombinedWith")]
    [InlineData(0.6, 0.7, "Either")]
    public void Validate_WithValidInput_ShouldPass(double probabilityA, double probabilityB, string function)
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = probabilityA,
            ProbabilityB = probabilityB,
            FunctionName = function
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("InvalidFunc")]
    [InlineData("CombinedWithout")]
    [InlineData("Eiter")]
    public void Validate_WithInvalidFunctionName_ShouldFail(string functionName)
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.2,
            ProbabilityB = 0.5,
            FunctionName = functionName
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FunctionName");
    }

    [Theory]
    [InlineData("CombinedWith")]
    [InlineData("combinedWith")]
    [InlineData("Combinedwith")]
    [InlineData("Either")]
    [InlineData("either")]
    public void Validate_WithValidFunctionNames_ShouldPass(string functionName)
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.2,
            ProbabilityB = 0.5,
            FunctionName = functionName
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithBoundaryValues_ShouldPass()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.01,
            ProbabilityB = 0.99,
            FunctionName = "Either"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = -0.01,
            ProbabilityB = 1.01,
            FunctionName = ""
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(4);
    }
}
