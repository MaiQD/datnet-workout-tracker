using Microsoft.AspNetCore.Mvc;
using dotFitness.Api.Infrastructure;
using dotFitness.Api.Infrastructure.Metrics;

namespace dotFitness.Api.Controllers;

/// <summary>
/// Controller for exposing module metrics and performance data
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(ILogger<MetricsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets overall module performance summary
    /// </summary>
    /// <returns>Performance summary for all modules</returns>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ModulePerformanceSummary), 200)]
    public IActionResult GetPerformanceSummary()
    {
        try
        {
            var summary = ModuleMetrics.GetPerformanceSummary();
            _logger.LogDebug("Retrieved module performance summary");
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving module performance summary");
            return StatusCode(500, "Error retrieving performance summary");
        }
    }

    /// <summary>
    /// Gets detailed metrics for all modules
    /// </summary>
    /// <returns>Detailed metrics for all modules</returns>
    [HttpGet("modules")]
    [ProducesResponseType(typeof(Dictionary<string, ModuleRegistrationMetrics>), 200)]
    public IActionResult GetAllModuleMetrics()
    {
        try
        {
            var metrics = ModuleMetrics.GetAllMetrics();
            _logger.LogDebug("Retrieved metrics for {ModuleCount} modules", metrics.Count);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving module metrics");
            return StatusCode(500, "Error retrieving module metrics");
        }
    }

    /// <summary>
    /// Gets detailed metrics for a specific module
    /// </summary>
    /// <param name="moduleName">Name of the module</param>
    /// <returns>Detailed metrics for the specified module</returns>
    [HttpGet("modules/{moduleName}")]
    [ProducesResponseType(typeof(ModuleRegistrationMetrics), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetModuleMetrics(string moduleName)
    {
        try
        {
            var metrics = ModuleMetrics.GetModuleMetrics(moduleName);
            if (metrics == null)
            {
                _logger.LogWarning("Module metrics not found for: {ModuleName}", moduleName);
                return NotFound($"Module metrics not found for: {moduleName}");
            }

            _logger.LogDebug("Retrieved metrics for module: {ModuleName}", moduleName);
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metrics for module: {ModuleName}", moduleName);
            return StatusCode(500, "Error retrieving module metrics");
        }
    }

    /// <summary>
    /// Clears all metrics (useful for testing and debugging)
    /// </summary>
    /// <returns>Success response</returns>
    [HttpDelete("clear")]
    [ProducesResponseType(200)]
    public IActionResult ClearMetrics()
    {
        try
        {
            ModuleMetrics.ClearMetrics();
            _logger.LogInformation("All module metrics cleared");
            return Ok("All module metrics cleared successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing module metrics");
            return StatusCode(500, "Error clearing module metrics");
        }
    }
} 