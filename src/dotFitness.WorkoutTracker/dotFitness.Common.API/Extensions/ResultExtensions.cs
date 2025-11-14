using dotFitness.Common.Results;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace dotFitness.Common.API.Extensions;

/// <summary>
/// Extension methods for mapping Result pattern to FastEndpoints responses
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Sends an appropriate HTTP response based on the Result pattern
    /// </summary>
    public static async Task SendResultAsync<TRequest, TResponse>(this Endpoint<TRequest, TResponse> endpoint, Result<TResponse> result, CancellationToken cancellationToken = default)
        where TRequest : notnull
        where TResponse : notnull
    {
        if (result.HasValue)
        {
            // Use Response property assignment for automatic 200 OK
            endpoint.Response = result.Value;
        }
        else
        {
            // Determine status code based on error message
            var statusCode = DetermineStatusCode(result.Error);
            endpoint.HttpContext.Response.StatusCode = statusCode;
            await endpoint.HttpContext.Response.WriteAsJsonAsync(
                new { error = result.Error ?? "An error occurred" },
                cancellationToken);
        }
    }

    /// <summary>
    /// Sends a 201 Created response with the result value
    /// </summary>
    public static async Task SendCreatedResultAsync<TRequest, TResponse>(this Endpoint<TRequest, TResponse> endpoint, Result<TResponse> result, string location, CancellationToken cancellationToken = default)
        where TRequest : notnull
        where TResponse : notnull
    {
        if (result.HasValue)
        {
            endpoint.HttpContext.Response.StatusCode = StatusCodes.Status201Created;
            endpoint.HttpContext.Response.Headers.Location = location;
            await endpoint.HttpContext.Response.WriteAsJsonAsync(result.Value, cancellationToken);
        }
        else
        {
            var statusCode = DetermineStatusCode(result.Error);
            endpoint.HttpContext.Response.StatusCode = statusCode;
            await endpoint.HttpContext.Response.WriteAsJsonAsync(
                new { error = result.Error ?? "An error occurred" },
                cancellationToken);
        }
    }

    private static int DetermineStatusCode(string? error)
    {
        if (string.IsNullOrEmpty(error))
            return StatusCodes.Status400BadRequest;

        var errorLower = error.ToLowerInvariant();
        
        if (errorLower.Contains("not found"))
            return StatusCodes.Status404NotFound;
        
        if (errorLower.Contains("already exists") || errorLower.Contains("conflict"))
            return StatusCodes.Status409Conflict;
        
        return StatusCodes.Status400BadRequest;
    }
}

