namespace Redington.Calculator.Infrastructure.Functions;

using Redington.Calculator.Domain.Interfaces;

/// <summary>
/// Represents a calculation function that computes the probability of either of two independent events occurring.
/// </summary>
/// <remarks>This function is used to calculate the combined probability of two independent events, A and B,
/// occurring. The formula used is: (probabilityA + probabilityB) - (probabilityA * probabilityB).</remarks>
public class EitherFunction : ICalculationFunction
{
    public string Name => "Either";

    public double Execute(double probabilityA, double probabilityB)
    {
        return (probabilityA + probabilityB) - (probabilityA * probabilityB);
    }
}
