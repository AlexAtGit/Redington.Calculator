namespace Redington.Calculator.Domain.Interfaces;

using Redington.Calculator.Domain.Models;

/// <summary>
/// Defines a service for performing calculations asynchronously.
/// </summary>
/// <remarks>This interface provides a method to execute calculations based on the specified request parameters.
/// Implementations should handle the calculation logic and return the results encapsulated in a <see
/// cref="CalculationResponse"/>.</remarks>
public interface ICalculationService
{
    Task<CalculationResponse> CalculateAsync(CalculationRequest request, CancellationToken cancellationToken = default);
}
