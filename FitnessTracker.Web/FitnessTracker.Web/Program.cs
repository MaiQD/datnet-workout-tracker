using FitnessTracker.Web.Client.Pages;
using FitnessTracker.Web.Components;
using FitnessTracker.RazorComponents.Services;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add configuration to access appsettings.json from RazorComponents
var razorComponentsConfig = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "FitnessTracker.RazorComponents", "appsettings.json"))
    .Build();

// Merge with web app configuration
builder.Configuration.AddConfiguration(razorComponentsConfig);

// Register MongoDB service with configuration
builder.Services.AddSingleton<MongoDBService>(provider => new MongoDBService(builder.Configuration));

// Register Syncfusion Blazor services
builder.Services.AddSyncfusionBlazor();
// Register Syncfusion license
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(razorComponentsConfig["Syncfusion:LicenseKey"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(FitnessTracker.Web.Client._Imports).Assembly)
    .AddAdditionalAssemblies(typeof(FitnessTracker.RazorComponents.Services.MongoDBService).Assembly);

app.Run();
