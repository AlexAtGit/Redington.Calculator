namespace Redington.Calculator.API.Validation;

using FluentValidation;
using Microsoft.Extensions.Options;
using Redington.Calculator.Domain.Models;

/// <summary>
/// Validates a calculation request.
/// </summary>
public class CalculationRequestValidator : AbstractValidator<CalculationRequest>
{
    public CalculationRequestValidator(IOptions<Constants> options)
    {
        var constants = options.Value;

        RuleFor(x => x.ProbabilityA)
            .InclusiveBetween(constants.MinProbabilityValue, constants.MaxProbabilityValue)
            .WithMessage($"ProbabilityA must be between {constants.MinProbabilityValue} and {constants.MaxProbabilityValue}");

        RuleFor(x => x.ProbabilityB)
            .InclusiveBetween(constants.MinProbabilityValue, constants.MaxProbabilityValue)
            .WithMessage($"ProbabilityB must be between {constants.MinProbabilityValue} and {constants.MaxProbabilityValue}");

        RuleFor(x => x.FunctionName)
            .NotEmpty()
            .WithMessage("FunctionName is required")
            .Must(f => new[] { "CombinedWith", "Either" }
                .Contains(f, StringComparer.OrdinalIgnoreCase))
            .WithMessage("FunctionName must be one of: CombinedWith, Either");
    }
}
