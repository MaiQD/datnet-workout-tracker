using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Syncfusion.Blazor;
using DatNetWorkoutTracker.Shared.Extensions;
using DatNetWorkoutTracker.Users.Extensions;
using DatNetWorkoutTracker.Exercises.Extensions;
using DatNetWorkoutTracker.Workouts.Extensions;
using DatNetWorkoutTracker.Routines.Extensions;
using DatNetWorkoutTracker.Analytics.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

// Add Syncfusion Blazor service
builder.Services.AddSyncfusionBlazor();

// Register Syncfusion license
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["SyncfusionLicense"]);

// Add shared services
builder.Services.AddSharedServices(builder.Configuration);

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleOAuth:ClientId"]!;
    options.ClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"]!;
    options.SaveTokens = true;
});

// Add authorization
builder.Services.AddAuthorization();

// Add modules
builder.Services.AddUsersModule();
builder.Services.AddExercisesModule();
builder.Services.AddWorkoutsModule();
builder.Services.AddRoutinesModule();
builder.Services.AddAnalyticsModule();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();
