using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dotFitness.ModuleContracts;

public interface IModuleInstaller
{
    public static string ModuleName;
    void InstallServices(IServiceCollection services, IConfiguration configuration, List<Assembly> assemblies);
}