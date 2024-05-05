using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Stores;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

if (builder.Configuration.GetRequiredSection(nameof(StellarDsClientSettings)).Get<StellarDsClientSettings>() is not { } stellarDsClientSettings)
{
    Debug.WriteLine("Unable to retrieve the StellarDsClientSettings from appsettings.json");

    return;
}

builder.Services.AddSingleton(stellarDsClientSettings);

builder.Services.AddHttpClient(stellarDsClientSettings.ApiSettings.Name, httpClient =>
{
    httpClient.BaseAddress = new Uri(stellarDsClientSettings.ApiSettings.BaseAddress);
});

builder.Services.AddHttpClient(stellarDsClientSettings.OAuthSettings.Name, httpClient =>
{
    httpClient.BaseAddress = new Uri(stellarDsClientSettings.OAuthSettings.BaseAddress);
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

builder.Services.AddScoped<OAuthAccessTokenProvider>();
builder.Services.AddScoped<ReadonlyAccessTokenProvider>();
builder.Services.AddScoped<DataApiService<OAuthAccessTokenProvider>>();
builder.Services.AddScoped<DataApiService<ReadonlyAccessTokenProvider>>();

builder.Services.AddScoped<SchemaApiService<OAuthAccessTokenProvider>>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

//Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseExceptionHandler("/Error/HandleException");


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "toDo",
    pattern: "{controller=ToDo}/{listId}/{action=Index}/{id?}");

app.Run();