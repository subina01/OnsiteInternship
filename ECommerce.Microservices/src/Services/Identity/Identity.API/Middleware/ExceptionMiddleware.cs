using FluentValidation;
using Identity.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Identity.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            FluentValidation.ValidationException validationEx => new Dictionary<string, object?>
            {
                ["StatusCode"] = (int)HttpStatusCode.BadRequest,
                ["Message"] = "Validation failed",
                ["Errors"] = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            },

            KeyNotFoundException => new Dictionary<string, object?>
            {
                ["StatusCode"] = (int)HttpStatusCode.NotFound,
                ["Message"] = exception.Message
            },

            UnauthorizedAccessException => new Dictionary<string, object?>
            {
                ["StatusCode"] = (int)HttpStatusCode.Unauthorized,
                ["Message"] = exception.Message
            },

            _ => new Dictionary<string, object?>
            {
                ["StatusCode"] = (int)HttpStatusCode.InternalServerError,
                ["Message"] = "An error occurred while processing your request"
            }
        };

        context.Response.StatusCode = (int)response["StatusCode"]!;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}