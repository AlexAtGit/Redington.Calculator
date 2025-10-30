namespace Redington.Calculator.Domain.Models;

/// <summary>
/// Represents the input to a calculation operation, including the probabilities and the function to be used.
/// </summary>
public class CalculationRequest
{
    public double ProbabilityA { get; set; }
    public double ProbabilityB { get; set; }
    public string FunctionName { get; set; } = string.Empty;
}
