using FastEndpoints.Testing;

namespace dotFitness.Api.Tests.Infrastructure;

/// <summary>
/// Application fixture for FastEndpoints testing
/// </summary>
public class AppFixture : AppFixture<Program>
{
    protected override Task SetupAsync()
    {
        // Any setup needed before tests run
        return Task.CompletedTask;
    }

    protected override Task TearDownAsync()
    {
        // Any cleanup needed after tests run
        return Task.CompletedTask;
    }
}

