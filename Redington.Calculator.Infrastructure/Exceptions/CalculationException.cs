namespace Redington.Calculator.Infrastructure.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid probability value is supplied.
/// </summary>
/// <remarks>This exception is typically thrown when a probability value is not between 0 and 1.</remarks>
public class CalculationException : Exception
{
    public CalculationException(string message) : base(message) { }

    public CalculationException(string message, Exception innerException)
        : base(message, innerException) { }
}
