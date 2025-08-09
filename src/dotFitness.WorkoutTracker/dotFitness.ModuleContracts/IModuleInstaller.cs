using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace dotFitness.ModuleContracts;

public interface IModuleInstaller
{
    void InstallServices(IServiceCollection services, IConfiguration configuration);
    void ConfigureIndexes(IMongoDatabase database);
    void SeedData(IMongoDatabase database);
}
