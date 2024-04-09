using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Generator.Models;
using StellarDsClient.Generator.Attributes;
using StellarDsClient.Generator.Extensions;
using System.Reflection;
using StellarDsClient.Sdk.Settings;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StellarDsClient.Generator.Helpers;
using StellarDsClient.Sdk;
using StellarDsClient.Generator.Providers;

//TODO: rename the project to 'builder'

var jsonWebTokenHandler = new JsonWebTokenHandler();

var localhostPort = AppSettingsHelpers.RequestLocalhostPort();

var oAuthSettings = AppSettingsHelpers.RequestOAuthSettings(localhostPort);
var apiSettings = AppSettingsHelpers.RequestApiSettings();

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel
builder.WebHost.UseKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(localhostPort, listenOptions =>
    {
        listenOptions.UseHttps(); // This will use the ASP.NET Core development certificate
    });
});

// Add services
builder.Services.AddScoped<OAuthApiService>();

builder.Services.AddSingleton(new AccessTokenProvider());
builder.Services.AddScoped<SchemaApiService<AccessTokenProvider>>();

builder.Services.AddSingleton(apiSettings);
builder.Services.AddSingleton(oAuthSettings);

builder.Services.AddHttpClient(apiSettings.Name, httpClient =>
{
    httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
});

var app = builder.Build();

app.MapGet("/oauth/oauthcallback", async context =>
{
    if (context.RequestServices.GetService<OAuthApiService>() is not { } oAuthApiService)
    {
        return;
    }

    var code = context.Request.Query["code"].ToString();
    var state = context.Request.Query["state"]; // todo?!

    if (string.IsNullOrWhiteSpace(code.ToString()))
    {
        return;
    }

    var tokens = await oAuthApiService.GetTokensAsync(code);

    if (tokens?.AccessToken is not { } accessToken)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Failed to obtain the access token.");

        return;
    }

    try
    {
        jsonWebTokenHandler.ReadJsonWebToken(tokens.AccessToken);
        await context.Response.WriteAsync("The access token is valid. You can close the browser.");
    }
    catch (Exception exception)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync($"The access token could not be retrieved: {exception.Message}");

        return;
    }

    if (context.RequestServices.GetService<AccessTokenProvider>() is not { } accessTokenProvider)
    {
        return;
    }
    
    accessTokenProvider.Set(accessToken);
    
    if (context.RequestServices.GetService<SchemaApiService<AccessTokenProvider>>() is not { } schemaApiService)
    {
        return;
    }

    var tableSettings = await schemaApiService.BuildDatabase();
    
    AppSettingsHelpers.WriteAppSettings(tableSettings, apiSettings, oAuthSettings);
});

// Open the system's default browser to the OAuth URL
var oauthUrl = $"https://stellards.io/oauth?client_id={oAuthSettings.ClientId}&redirect_uri={oAuthSettings.RedirectUri}&response_type=code";
Process.Start(new ProcessStartInfo
{
    FileName = oauthUrl,
    UseShellExecute = true
});

app.Run();