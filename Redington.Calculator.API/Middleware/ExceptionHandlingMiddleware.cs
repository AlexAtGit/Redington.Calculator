namespace Redington.Calculator.API.Middleware;

using Redington.Calculator.Infrastructure.Exceptions;
using Redington.Calculator.Domain.Models;

using System;
using System.Text.Json;

/// <summary>
/// Provides middleware for handling exceptions globally.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = exception switch
        {
            InvalidFunctionException => new ErrorResponse
            {
                Message = exception.Message,
                StatusCode = StatusCodes.Status400BadRequest
            },
            CalculationException => new ErrorResponse
            {
                Message = exception.Message,
                StatusCode = StatusCodes.Status400BadRequest
            },
            _ => new ErrorResponse
            {
                Message = "An internal server error occurred",
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, jsonOptions));
    }
}