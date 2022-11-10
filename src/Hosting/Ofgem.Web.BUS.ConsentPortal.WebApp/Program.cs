using Ofgem.Lib.BUS.Logging;
using Ofgem.Web.BUS.ConsentPortal.Core;
using Ofgem.API.BUS.PropertyConsents.Client;
using Azure.Identity;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(
    options => options.Connect(new Uri($"https://{builder.Configuration["AppConfig:Name"]}.azconfig.io"), new DefaultAzureCredential())
                      .UseFeatureFlags()
                      .Select(KeyFilter.Any, LabelFilter.Null)
                      .Select(KeyFilter.Any, builder.Configuration["AppConfig:LabelFilter"])
                      .ConfigureRefresh(refresh =>
                      {
                          refresh.Register("Bus:SentinelKey", true).SetCacheExpiration(TimeSpan.FromSeconds(30));
                      }));

// Add services to the container.
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AddAreaPageRoute("Verify",
           "/verify/", "/consent/halt");
});

// Azure Key Vault configuration
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential());

builder.Services.AddServiceConfigurations(builder.Configuration);
builder.Services.AddPropertyConsentClient(builder.Configuration, "PropertyConsentApiUrl");
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAzureAppConfiguration();
builder.Services.AddFeatureManagement();

// Logging
builder.Services.AddOfgemCloudApplicationInsightsTelemetry();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAzureAppConfiguration();

//for errors with no response body e.g 404 mainly
app.UseStatusCodePagesWithReExecute("/Shared/RoutingError/", "?statusCode={0}");
//for errors with response body e.g failed API calls mainly 500
app.UseExceptionHandler("/Shared/InternalError");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.UseTelemetryMiddleware();
app.UseSession();
app.Run();
