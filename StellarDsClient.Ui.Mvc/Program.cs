using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Stores;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using StellarDsClient.Builder.Library;
using StellarDsClient.Sdk.Models;
using System.Text.Json;


#if  DEBUG 
var stellarDsSettings = await new DbBuilder().Run(args);
#else
//todo: dispose the configurationbuilder? or is it always the same instance?
var stellarDsSettings = new ConfigurationBuilder().AddJsonFile("appsettings.StellarDs.json", false).Build().Get<StellarDsSettings>() ?? throw new NullReferenceException("Unable to get the StellarDsSettings");
#endif

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var apiSettings = stellarDsSettings.ApiSettings;
builder.Services.AddSingleton(apiSettings);

var oAuthSettings = stellarDsSettings.OAuthSettings;
builder.Services.AddSingleton(oAuthSettings);

var cookieSettings = builder.Configuration.GetSection(nameof(CookieSettings)).Get<CookieSettings>();
if (cookieSettings is null)
{
    Debug.WriteLine($"Unable to create {nameof(CookieSettings)}");
    return;
}
builder.Services.AddSingleton(cookieSettings);

builder.Services.AddSingleton(stellarDsSettings.TableSettings);


builder.Services.AddHttpClient(apiSettings.Name, httpClient =>
{
    httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
});

builder.Services.AddHttpClient(oAuthSettings.Name, httpClient =>
{
    httpClient.BaseAddress = new Uri(oAuthSettings.BaseAddress);
});


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JsonWebTokenHandler>(); //TODO why is this added as a service? there is no need for it in the console app?!

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ReturnUrlParameter = "returnUrl";
        options.LoginPath = new PathString("/oauth");
        options.SlidingExpiration = false;
    });


builder.Services.AddScoped<OAuthTokenStore>();

builder.Services.AddScoped<OAuthApiService>();

builder.Services.AddScoped<OAuthTokenProvider>();
builder.Services.AddScoped<ReadonlyAccessTokenProvider>();
builder.Services.AddScoped<DataApiService<OAuthTokenProvider>>();
builder.Services.AddScoped<DataApiService<ReadonlyAccessTokenProvider>>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "task",
    pattern: "{controller=ToDo}/{listId}/{action=Index}/{id?}");

app.Run();