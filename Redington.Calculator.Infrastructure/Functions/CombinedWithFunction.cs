namespace Redington.Calculator.Infrastructure.Functions;

using Redington.Calculator.Domain.Interfaces;

/// <summary>
/// Represents a calculation function that computes the combined probability of two independent events.
/// </summary>
/// <remarks>This class implements the <see cref="ICalculationFunction"/> interface and provides a method to
/// calculate the probability of both events occurring together by multiplying their individual probabilities.</remarks>
public class CombinedWithFunction : ICalculationFunction
{
    public string Name => "CombinedWith";

    public double Execute(double probabilityA, double probabilityB)
    {
        return probabilityA * probabilityB;
    }
}
