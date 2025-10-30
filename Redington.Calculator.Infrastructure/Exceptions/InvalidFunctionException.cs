namespace Redington.Calculator.Infrastructure.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an unsupported function is encountered during a calculation.
/// </summary>
/// <remarks>This exception is typically thrown when a function name provided to a calculation operation is not
/// recognized or supported by the system.</remarks>
public class InvalidFunctionException : CalculationException
{
    public InvalidFunctionException(string functionName)
        : base($"Function '{functionName}' is not supported") { }
}
