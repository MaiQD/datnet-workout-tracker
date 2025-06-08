using Microsoft.Extensions.Logging;
using FitnessTracker.RazorComponents.Services;
using Syncfusion.Blazor;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.IO;
using Plugin.Maui.Calendar;

namespace FitnessTracker.MauiClient;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		// Load configuration
		var assembly = Assembly.GetExecutingAssembly();
		using var stream = assembly.GetManifestResourceStream("FitnessTracker.MauiClient.appsettings.json");
		
		// Create a temporary configuration for development scenarios
		var tempConfig = new ConfigurationBuilder()
			.AddJsonStream(stream ?? new MemoryStream())
			.Build();
			
		// Register MongoDBService with the proper configuration
		builder.Services.AddSingleton<MongoDBService>(provider =>
		{
			return new MongoDBService(tempConfig);
		});
		
		// Register Syncfusion Blazor services
		builder.Services.AddSyncfusionBlazor();
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(tempConfig["Syncfusion:LicenseKey"]);
		
		// Add the MAUI Calendar Plugin
		builder.UseMauiCalendar();

		return builder.Build();
	}
}
