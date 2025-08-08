using Microsoft.Extensions.Options;
using dotFitness.Api.Infrastructure.Settings;

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
    /// Configures health check endpoints with custom response writers
    /// </summary>
    public static WebApplication ConfigureHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration.ToString()
                    })
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        });

        app.MapHealthChecks("/health/modules", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("module"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    modules = report.Entries.Where(e => e.Value.Tags.Contains("module")).Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration.ToString()
                    })
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        });

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