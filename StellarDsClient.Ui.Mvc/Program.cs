using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Stores;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var apiSettings = builder.Configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>();
if (apiSettings is null)
{
    Debug.WriteLine($"Unable to create {nameof(ApiSettings)}");
    return;
}
builder.Services.AddSingleton(apiSettings);

var oAuthSettings = builder.Configuration.GetSection(nameof(OAuthSettings)).Get<OAuthSettings>();
if (oAuthSettings is null)
{
    Debug.WriteLine($"Unable to create {nameof(OAuthSettings)}");
    return;
}
builder.Services.AddSingleton(oAuthSettings);

var cookieSettings = builder.Configuration.GetSection(nameof(CookieSettings)).Get<CookieSettings>();
if (cookieSettings is null)
{
    Debug.WriteLine($"Unable to create {nameof(CookieSettings)}");
    return;
}
builder.Services.AddSingleton(cookieSettings);

var tableSettings = builder.Configuration.GetSection(nameof(TableSettings)).Get<TableSettings>();
if (tableSettings is null)
{
    Debug.WriteLine($"Unable to create {nameof(TableSettings)}");
    return;
}
builder.Services.AddSingleton(tableSettings);

builder.Services.AddHttpClient(apiSettings.Name, httpClient =>
{
    httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
});

builder.Services.AddHttpClient(oAuthSettings.Name, httpClient =>
{
    httpClient.BaseAddress = new Uri(oAuthSettings.BaseAddress);
});


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JsonWebTokenHandler>(); //TODO why is this added as a service? there is no need for it in console app?!

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ReturnUrlParameter = "returnUrl";
        options.LoginPath = new PathString("/oauth");
        options.SlidingExpiration = false;
    });


builder.Services.AddScoped<IOAuthTokenStore, OAuthTokenStore>();

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
    pattern: "{controller=Task}/{listId}/{action=Index}/{id?}");

app.Run();