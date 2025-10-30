namespace Redington.Calculator.Infrastructure.Services;

using Redington.Calculator.Infrastructure.Exceptions;
using Redington.Calculator.Domain.Interfaces;
using Redington.Calculator.Domain.Models;

using Microsoft.Extensions.Logging;

/// <summary>
/// Provides calculation services using a collection of calculation functions.
/// </summary>
/// <remarks>This service is responsible for executing the specified calculation function with given probabilities.
/// It logs the calculation process and handles exceptions that may occur during execution.</remarks>
public class CalculationService : ICalculationService
{
    private readonly IEnumerable<ICalculationFunction> _functions;
    private readonly ILogger<CalculationService> _logger;

    public CalculationService(IEnumerable<ICalculationFunction> functions, ILogger<CalculationService> logger)
    {
        _functions = functions ?? throw new ArgumentNullException(nameof(functions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<CalculationResponse> CalculateAsync(CalculationRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogInformation(
            "Calculating {FunctionName} with values {ProbabilityA} and {ProbabilityB}",
            request.FunctionName,
            request.ProbabilityA,
            request.ProbabilityB);

        // Identify the function to be used. Note the operation is case-insensitive.
        // Alternatively, we could have used the factory pattern to create the function
        var function = _functions.FirstOrDefault(
            f => f.Name.Equals(request.FunctionName, StringComparison.OrdinalIgnoreCase));

        if (function == null)
        {
            _logger.LogWarning("Invalid function requested: {FunctionName}", request.FunctionName);
            throw new InvalidFunctionException(request.FunctionName);
        }

        try
        {
            // Perform the calculation
            var result = function.Execute(request.ProbabilityA, request.ProbabilityB);

            _logger.LogInformation(
                "Calculation successful: {FunctionName}({ProbabilityA}, {ProbabilityB}) = {Result}",
                request.FunctionName,
                request.ProbabilityA,
                request.ProbabilityB,
                result);

            return Task.FromResult(new CalculationResponse
            {
                Result = result,
                Function = function.Name,
                CalculatedAt = DateTime.UtcNow
            });
        }
        catch (CalculationException ex)
        {
            _logger.LogError(ex, "Calculation error occurred");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during calculation");
            throw new CalculationException("An unexpected error occurred during calculation", ex);
        }
    }
}
