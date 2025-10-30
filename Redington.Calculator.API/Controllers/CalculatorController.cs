namespace Redington.Calculator.API.Controllers;

using Redington.Calculator.Domain.Interfaces;
using Redington.Calculator.Domain.Models;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for handling calculation requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly ICalculationService _calculationService;
    private readonly ILogger<CalculatorController> _logger;

    public CalculatorController(
        ICalculationService calculationService,
        ILogger<CalculatorController> logger)
    {
        _calculationService = calculationService;
        _logger = logger;
    }

    /// <summary>
    /// Performs a calculation based on two numbers and a function
    /// </summary>
    /// <param name="request">The calculation request containing two numbers and a function</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the calculation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CalculationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CalculationResponse>> Calculate(
        [FromBody] CalculationRequest calculationRequest,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received calculation request");

        // Ideally, we should use the Mediator pattern to decouple the controller from the service layer.
        var result = await _calculationService.CalculateAsync(
            calculationRequest,
            cancellationToken);

        return Ok(result);
    }
}
