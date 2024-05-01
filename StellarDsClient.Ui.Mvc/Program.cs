using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using SQLitePCL;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Services;
using StellarDsClient.Ui.Mvc.Stores;
using System.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration.GetSection(nameof(CookieSettings)).Get<CookieSettings>() is not { } cookieSettings)
{
    Debug.WriteLine("Unable to retrieve the CookieSettings from appsettings.json");

    return;
}

if (builder.Configuration.GetSection(nameof(OAuthSettings)).Get<OAuthSettings>() is not { } oAuthSettings)
{
    Debug.WriteLine("Unable to retrieve the OAuthSettings from appsettings.json");

    return;
}

if (builder.Configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>() is not { } apiSettings)
{
    Debug.WriteLine("Unable to retrieve the ApiSettings from appsettings.json");

    return;
}

if (builder.Configuration.GetSection(nameof(OAuthCredentials)).Get<OAuthCredentials>() is not { } oAuthCredentials)
{
    Debug.WriteLine("Unable to retrieve the OAuthCredentials from appsettings.json");

    return;
}

if (builder.Configuration.GetSection(nameof(ApiCredentials)).Get<ApiCredentials>() is not { } apiCredentials)
{
    Debug.WriteLine("Unable to retrieve the ApiCredentials from appsettings.json");

    return;
}


Batteries_V2.Init();

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<SqliteService>();

builder.Services.AddSingleton(apiCredentials);

builder.Services.AddSingleton(oAuthCredentials);

builder.Services.AddSingleton(cookieSettings);

builder.Services.AddSingleton(apiSettings);

builder.Services.AddSingleton(oAuthSettings);

builder.Services.AddSingleton(new TableSettings());

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

builder.Services.AddScoped<OAuthAccessTokenProvider>();
builder.Services.AddScoped<ReadonlyAccessTokenProvider>();
builder.Services.AddScoped<DataApiService<OAuthAccessTokenProvider>>();
builder.Services.AddScoped<DataApiService<ReadonlyAccessTokenProvider>>();

builder.Services.AddScoped<SchemaApiService<OAuthAccessTokenProvider>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var sqliteService = app.Services.GetService<SqliteService>();
if(sqliteService is null)
{
    Debug.WriteLine("Unable to get the SqliteService from the ServiceCollection");
    return;
};

sqliteService.EnsureDatabase();

var tableSettings = app.Services.GetService<TableSettings>();
if(tableSettings is null)
{
    Debug.WriteLine("Unable to get the TableSettings from the ServiceCollection.");
    return;
}

var listTableId = sqliteService.GetTableId(nameof(List));
tableSettings.Add(nameof(List), listTableId ?? 0);

var toDoTableId = sqliteService.GetTableId(nameof(ToDo));
tableSettings.Add(nameof(ToDo), toDoTableId ?? 0);


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "toDo",
    pattern: "{controller=ToDo}/{listId}/{action=Index}/{id?}");

app.Run();