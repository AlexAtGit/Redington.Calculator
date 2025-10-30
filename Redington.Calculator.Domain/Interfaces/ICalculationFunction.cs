namespace Redington.Calculator.Domain.Interfaces;

/// <summary>
/// Represents a calculation function that operates on two probability values.
/// </summary>
/// <remarks>Implementations of this interface define specific calculation logic using the provided
/// probabilities.</remarks>
public interface ICalculationFunction
{
    string Name { get; }

    double Execute(double probabilityA, double probabilityB);
}
