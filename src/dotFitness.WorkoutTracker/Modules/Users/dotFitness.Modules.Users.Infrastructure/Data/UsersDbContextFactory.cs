using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace dotFitness.Modules.Users.Infrastructure.Data;

public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
        
        // Priority order for getting connection string:
        // 1. Command line arguments
        // 2. Environment variable
        // 3. Default development connection string
        
        string? connectionString = null;
        
        // Check command line arguments first
        if (args.Length > 0)
        {
            // Look for --connection-string argument
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "--connection-string" || args[i] == "--conn")
                {
                    connectionString = args[i + 1];
                    break;
                }
            }
        }
        
        // Check environment variable if not found in args
        connectionString ??= Environment.GetEnvironmentVariable("DOTFITNESS_POSTGRESQL_CONNECTION");
        
        // Check for Aspire-generated connection string
        connectionString ??= Environment.GetEnvironmentVariable("ConnectionStrings__dotFitnessDb-pg");
        
        // Default development connection string as fallback (for non-Aspire scenarios)
        connectionString ??= "Host=localhost;Database=dotFitnessDb;Username=postgres;Password=password;Port=5432";

        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsHistoryTable("__EFMigrationsHistory", "users");
            options.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
        });

        return new UsersDbContext(optionsBuilder.Options);
    }
}
