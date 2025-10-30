namespace Redington.Calculator.Infrastructure.Tests.Services;

using Redington.Calculator.Domain.Interfaces;
using Redington.Calculator.Domain.Models;
using Redington.Calculator.Infrastructure.Functions;
using Redington.Calculator.Infrastructure.Exceptions;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Redington.Calculator.Infrastructure.Services;

public class CalculationServiceTests
{
    private readonly Mock<ILogger<CalculationService>> _loggerMock;
    private readonly List<ICalculationFunction> _functions;
    private readonly CalculationService _service;

    public CalculationServiceTests()
    {
        _loggerMock = new Mock<ILogger<CalculationService>>();
        _functions = new List<ICalculationFunction>
        {
            new CombinedWithFunction(),
            new EitherFunction()
        };
        _service = new CalculationService(_functions, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithNullFunctions_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new CalculationService(null!, _loggerMock.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("functions");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new CalculationService(_functions, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [Fact]
    public async Task CalculateAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act
        Func<Task> act = async () => await _service.CalculateAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Theory]
    [InlineData("CombinedWith", 0.5, 0.5, 0.25)]
    [InlineData("Either", 0.5, 0.5, 0.75)]
    public async Task CalculateAsync_WithValidRequest_ShouldReturnCorrectResult(
        string functionName, double probabilityA, double probabilityB, double expected)
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = probabilityA,
            ProbabilityB = probabilityB,
            FunctionName = functionName
        };

        // Act
        var result = await _service.CalculateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().Be(expected);
        result.Function.Should().Be(functionName);
        result.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("CombinedWith")]
    [InlineData("combinedWith")]
    [InlineData("Combinedwith")]
    [InlineData("combinedwith")]
    public async Task CalculateAsync_WithCaseInsensitiveFunction_ShouldWork(string functionName)
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.3,
            ProbabilityB = 0.9,
            FunctionName = functionName
        };

        // Act
        var result = await _service.CalculateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().Be(0.27);
    }

    [Fact]
    public async Task CalculateAsync_WithInvalidFunction_ShouldThrowInvalidFunctionException()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.4,
            ProbabilityB = 0.7,
            FunctionName = "InvalidFunc"
        };

        // Act
        Func<Task> act = async () => await _service.CalculateAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidFunctionException>()
            .WithMessage("Function 'InvalidFunc' is not supported");
    }

    [Fact]
    public async Task CalculateAsync_ShouldLogInformation()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.5,
            ProbabilityB = 0.4,
            FunctionName = "CombinedWith"
        };

        // Act
        await _service.CalculateAsync(request);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task CalculateAsync_WithInvalidFunction_ShouldLogWarning()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.3,
            ProbabilityB = 0.8,
            FunctionName = "InvalidFunc"
        };

        // Act
        try
        {
            await _service.CalculateAsync(request);
        }
        catch
        {
            // Expected exception
        }

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
