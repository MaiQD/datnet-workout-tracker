using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace dotFitness.Api.Infrastructure.Middleware;

public class GlobalErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalErrorHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Log additional context
        _logger.LogError(exception, 
            "Exception occurred during request to {RequestPath} with method {RequestMethod}. " +
            "TraceId: {TraceId}, User: {User}", 
            context.Request.Path, 
            context.Request.Method,
            context.TraceIdentifier,
            context.User?.Identity?.Name ?? "Anonymous");

        var errorResponse = exception switch
        {
            ValidationException validationEx => CreateValidationErrorResponse(validationEx, context.TraceIdentifier),

            UnauthorizedAccessException => CreateErrorResponse(
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                exception.Message,
                context.TraceIdentifier),

            ArgumentNullException => CreateErrorResponse(
                HttpStatusCode.BadRequest,
                "Bad Request",
                "A required parameter was not provided",
                context.TraceIdentifier),

            ArgumentException => CreateErrorResponse(
                HttpStatusCode.BadRequest,
                "Bad Request",
                exception.Message,
                context.TraceIdentifier),

            InvalidOperationException => CreateErrorResponse(
                HttpStatusCode.BadRequest,
                "Invalid Operation",
                exception.Message,
                context.TraceIdentifier),

            KeyNotFoundException => CreateErrorResponse(
                HttpStatusCode.NotFound,
                "Not Found",
                exception.Message,
                context.TraceIdentifier),

            NotImplementedException => CreateErrorResponse(
                HttpStatusCode.NotImplemented,
                "Not Implemented",
                "This feature is not yet implemented",
                context.TraceIdentifier),

            TimeoutException => CreateErrorResponse(
                HttpStatusCode.RequestTimeout,
                "Request Timeout",
                "The request timed out",
                context.TraceIdentifier),

            _ => CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An internal server error occurred",
                context.TraceIdentifier)
        };

        context.Response.StatusCode = (int)errorResponse.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse.Body, options);
        await context.Response.WriteAsync(jsonResponse);
    }

    private static ErrorResponse CreateErrorResponse(HttpStatusCode statusCode, string title, string detail, string? traceId = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = null
        };

        if (!string.IsNullOrEmpty(traceId))
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        return new ErrorResponse
        {
            StatusCode = statusCode,
            Body = problemDetails
        };
    }

    private static ErrorResponse CreateValidationErrorResponse(ValidationException validationException, string? traceId = null)
    {
        var problemDetails = new ValidationProblemDetails();
        problemDetails.Status = (int)HttpStatusCode.BadRequest;
        problemDetails.Title = "Validation Error";
        problemDetails.Detail = "One or more validation errors occurred";

        if (!string.IsNullOrEmpty(traceId))
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        foreach (var error in validationException.Errors)
        {
            if (problemDetails.Errors.ContainsKey(error.PropertyName))
            {
                var existingErrors = problemDetails.Errors[error.PropertyName].ToList();
                existingErrors.Add(error.ErrorMessage);
                problemDetails.Errors[error.PropertyName] = existingErrors.ToArray();
            }
            else
            {
                problemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
            }
        }

        return new ErrorResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Body = problemDetails
        };
    }

    private class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public ProblemDetails Body { get; set; } = null!;
    }
}
