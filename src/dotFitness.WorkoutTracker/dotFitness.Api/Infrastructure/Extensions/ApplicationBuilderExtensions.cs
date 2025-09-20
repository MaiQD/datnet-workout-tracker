using Microsoft.Extensions.Options;
using System.Text.Json;
using dotFitness.Api.Infrastructure.Settings;
using dotFitness.Api.Infrastructure.Middleware;

namespace dotFitness.Api.Infrastructure.Extensions;

/// <summary>
/// Extension methods for WebApplication to organize middleware pipeline
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures Swagger UI with OAuth2 settings for development environment
    /// </summary>
    public static WebApplication ConfigureSwaggerUi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "dotFitness API v1");
                c.RoutePrefix = string.Empty; // Serve Swagger UI at root URL
                
                // Configure OAuth2 settings for Google
                var googleOAuthSettings = app.Services.GetRequiredService<IOptions<GoogleOAuthSettings>>().Value;
                c.OAuthClientId(googleOAuthSettings.ClientId);
                c.OAuthClientSecret(googleOAuthSettings.ClientSecret);
                c.OAuthRealm("dotFitness");
                c.OAuthAppName("dotFitness API");
                c.OAuthScopeSeparator(" ");
                c.OAuthUsePkce();
            });
        }

        return app;
    }

    /// <summary>
    /// Configures the core middleware pipeline
    /// </summary>
    public static WebApplication ConfigureCoreMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Configures additional health check endpoints that complement Aspire's default endpoints
    /// Note: Aspire already maps /health and /alive endpoints, so we add module-specific endpoints
    /// </summary>
    public static WebApplication ConfigureHealthChecks(this WebApplication app)
    {
        // Detailed health check endpoint with custom JSON response (complements Aspire's /health)
        app.MapHealthChecks("/health/detailed", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    timestamp = DateTime.UtcNow,
                    totalDuration = report.TotalDuration.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration.ToString(),
                        tags = e.Value.Tags,
                        data = e.Value.Data
                    })
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
            }
        });

        // Module-specific health checks endpoint
        app.MapHealthChecks("/health/modules", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("module"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    timestamp = DateTime.UtcNow,
                    modules = report.Entries
                        .Where(e => e.Value.Tags.Contains("module"))
                        .Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration.ToString(),
                            data = e.Value.Data
                        })
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
            }
        });

        // Global/registry health checks endpoint
        app.MapHealthChecks("/health/global", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("global"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    timestamp = DateTime.UtcNow,
                    global = report.Entries
                        .Where(e => e.Value.Tags.Contains("global"))
                        .Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration.ToString(),
                            data = e.Value.Data
                        })
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
            }
        });

        return app;
    }

    /// <summary>
    /// Configures global error handling middleware
    /// </summary>
    public static WebApplication UseGlobalErrorHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalErrorHandlerMiddleware>();
        return app;
    }

    /// <summary>
    /// Configures application endpoints
    /// </summary>
    public static WebApplication ConfigureEndpoints(this WebApplication app)
    {
        app.MapControllers();
        return app;
    }
} 