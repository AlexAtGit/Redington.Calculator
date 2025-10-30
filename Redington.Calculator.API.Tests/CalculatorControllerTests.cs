namespace Redington.Calculation.API.Tests.Controllers;

using Redington.Calculator.API.Controllers;
using Redington.Calculator.Infrastructure.Exceptions;
using Redington.Calculator.Domain.Interfaces;
using Redington.Calculator.Domain.Models;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

public class CalculatorControllerTests
{
    private readonly Mock<ICalculationService> _calculationServiceMock;
    private readonly Mock<ILogger<CalculatorController>> _loggerMock;
    private readonly CalculatorController _controller;

    public CalculatorControllerTests()
    {
        _calculationServiceMock = new Mock<ICalculationService>();
        _loggerMock = new Mock<ILogger<CalculatorController>>();
        _controller = new CalculatorController(
            _calculationServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Calculate_WithValidRequest_ShouldReturnOkResultWithCorrectValue()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.2,
            ProbabilityB = 0.6,
            FunctionName = "CombinedWith"
        };

        var expectedResult = new CalculationResponse
        {
            Result = 0.12,
            Function = "CombinedWith",
            CalculatedAt = DateTime.UtcNow
        };

        _calculationServiceMock
            .Setup(x => x.CalculateAsync(It.IsAny<CalculationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Calculate(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<CalculationResponse>().Subject;
        response.Result.Should().Be(0.12);
    }

    [Fact]
    public async Task Calculate_ShouldCallCalculationServiceWithCorrectParameters()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.2,
            ProbabilityB = 0.7,
            FunctionName = "CombinedWith"
        };

        var expectedResult = new CalculationResponse
        {
            Result = 0.14,
            Function = "CombinedWith"
        };

        _calculationServiceMock
            .Setup(x => x.CalculateAsync(It.IsAny<CalculationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        await _controller.Calculate(request, CancellationToken.None);

        // Assert
        _calculationServiceMock.Verify(
            x => x.CalculateAsync(
                It.Is<CalculationRequest>(r =>
                    r.ProbabilityA == 0.2 &&
                    r.ProbabilityB == 0.7 &&
                    r.FunctionName == "CombinedWith"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("CombinedWith", 0.5, 0.5, 0.25)]
    [InlineData("Either", 0.5, 0.5, 0.75)]
    public async Task Calculate_WithDifferentFunctionss_ShouldReturnCorrectResults(
        string functionName, double probabilityA, double probabilityB, double expected)
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = probabilityA,
            ProbabilityB = probabilityB,
            FunctionName = functionName
        };

        var calculationResult = new CalculationResponse
        {
            Result = expected,
            Function = functionName
        };

        _calculationServiceMock
            .Setup(x => x.CalculateAsync(It.IsAny<CalculationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(calculationResult);

        // Act
        var result = await _controller.Calculate(request, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<CalculationResponse>().Subject;
        response.Result.Should().Be(expected);
    }

    [Fact]
    public async Task Calculate_WhenServiceThrowsInvalidFunctionException_ShouldPropagate()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.5,
            ProbabilityB = 0.4,
            FunctionName = "InvalidFunc"
        };

        _calculationServiceMock
            .Setup(x => x.CalculateAsync(It.IsAny<CalculationRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidFunctionException("InvalidFunc"));

        // Act
        Func<Task> act = async () => await _controller.Calculate(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidFunctionException>();
    }

    [Fact]
    public async Task Calculate_WhenServiceThrowsCalculationException_ShouldPropagate()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.5,
            ProbabilityB = -0.1,
            FunctionName = "CombineWith"
        };

        _calculationServiceMock
            .Setup(x => x.CalculateAsync(It.IsAny<CalculationRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CalculationException("CombineWith with negative probability is not allowed"));

        // Act
        Func<Task> act = async () => await _controller.Calculate(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<CalculationException>()
            .WithMessage("CombineWith with negative probability is not allowed");
    }

    [Fact]
    public async Task Calculate_ShouldLogInformation()
    {
        // Arrange
        var request = new CalculationRequest
        {
            ProbabilityA = 0.5,
            ProbabilityB = 0.4,
            FunctionName = "Either"
        };

        _calculationServiceMock
            .Setup(x => x.CalculateAsync(It.IsAny<CalculationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CalculationResponse { Result = 0.37 });

        // Act
        await _controller.Calculate(request, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
