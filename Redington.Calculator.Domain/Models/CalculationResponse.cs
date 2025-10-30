namespace Redington.Calculator.Domain.Models;

/// <summary>
/// Represents the response of a calculation operation, including the result, the function used, and the timestamp of
/// the calculation.
/// </summary>
public class CalculationResponse
{
    public double Result { get; set; }
    public string Function { get; set; } = string.Empty;
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}
