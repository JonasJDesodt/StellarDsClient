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

var jsonWebTokenHandler = new JsonWebTokenHandler();


//GET THE LOCALHOST PORT
Console.Write("Localhost port: ");
var localHostPort = Console.ReadLine();
if (!int.TryParse(localHostPort, out var port))
{
    Console.WriteLine("The provided localhost port is not a integer");

    return;
}

// GET THE CLIENT ID
Console.Write("Client Id: ");
var clientId = Console.ReadLine();
if (!Guid.TryParse(clientId, out var clientGuid))
{
    Console.WriteLine("The provided client id is not a valid GUID");

    return;
}

//GET THE CLIENT SECRET
Console.Write("Client secret: ");
var clientSecret = Console.ReadLine();
if (string.IsNullOrWhiteSpace(clientSecret))
{
    Console.WriteLine("Please provide a client secret");

    return;
}

//GET THE PROJECT ID
Console.Write("Project id: ");
var projectId = Console.ReadLine();

if (string.IsNullOrWhiteSpace(projectId))
{
    Console.WriteLine("Please provide an access token.");

    return;
}

if (!Guid.TryParse(projectId, out var projectGuid))
{
    Console.WriteLine("The provided project id is not a valid GUID");

    return;
}

//GET READONLY ACCESS TOKEN
Console.Write("Readonly access token: ");
var readOnlyAccessToken = Console.ReadLine();
if (string.IsNullOrWhiteSpace(readOnlyAccessToken))
{
    Console.WriteLine("Please provide an readonly access token.");

    return;
}

JsonWebToken readOnlyJsonWebAccessToken;

try
{
    readOnlyJsonWebAccessToken = jsonWebTokenHandler.ReadJsonWebToken(readOnlyAccessToken);
}
catch (Exception exception)
{
    Console.WriteLine(exception);

    return;
}



var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel
builder.WebHost.UseKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(port, listenOptions =>
    {
        listenOptions.UseHttps(); // This will use the ASP.NET Core development certificate
    });
});


// Add services
builder.Services.AddScoped<OAuthApiService>();

builder.Services.AddSingleton(new AccessTokenProvider());
builder.Services.AddScoped<SchemaApiService<AccessTokenProvider>>();

var apiSettings = new ApiSettings
{
    BaseAddress = "https://api.stellards.io",
    Name = "StellarDs",
    Project = projectId.ToString(),
    ReadOnlyToken = readOnlyAccessToken.ToString(),
    Version = "v1"
};
builder.Services.AddSingleton(apiSettings);

builder.Services.AddSingleton(new OAuthSettings
{
    BaseAddress = "https://stellards.io",
    ClientId = clientId.ToString(),
    ClientSecret = clientSecret,
    Name = "OAuth",
    RedirectUri = $"https://localhost:{port}/oauth/oauthcallback",
});

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
        await context.Response.WriteAsync($"Invalid access token: {exception.Message}");

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

    var tablesStellarDsResult = await schemaApiService.FindTables();

    if (tablesStellarDsResult.Data is not { } tableResults)
    {
        Console.WriteLine("Unable to retrieve table metadata:");

        foreach (var message in tablesStellarDsResult.Messages)
        {
            Console.WriteLine(message.Code + ": " + message.Message);
        }

        return;
    }

    //CREATE THE LIST TABLE
    if (tableResults.GetMetadata("list") is { } oldListMetadata)
    {
        await schemaApiService.DeleteTable(oldListMetadata.Id);
    }

    var listMetadataStellarDsResult = await schemaApiService.CreateTable("list", true);

    if (listMetadataStellarDsResult.Data is not { } newListMetadata)
    {
        Console.WriteLine("Failed to create the list table: ");
        foreach (var message in listMetadataStellarDsResult.Messages)
        {
            Console.WriteLine(message.Code + ": " + message.Message);
        }

        return;
    }

    foreach (var property in typeof(ListPropertyMapper).GetProperties())
    {
        var stellarDsType = property.GetCustomAttribute<StellarDsType>()?.Name ?? property.PropertyType.ToString();

        var fieldMetadataStellarDsResult = await schemaApiService.CreateField(newListMetadata.Id, property.Name, stellarDsType);

        if (fieldMetadataStellarDsResult.Data is not null)
        {
            continue;
        }

        Console.WriteLine("Failed to create field for the list table: ");
        foreach (var message in fieldMetadataStellarDsResult.Messages)
        {
            Console.WriteLine(message.Code + ": " + message.Message);
        }

        return;
    }

    //CREATE THE TASK TABLE
    if (tableResults.GetMetadata("task") is { } oldTaskMetadata)
    {
        await schemaApiService.DeleteTable(oldTaskMetadata.Id);
    }

    var taskMetadataStellarDsResult = await schemaApiService.CreateTable("task", true);

    if (taskMetadataStellarDsResult.Data is not { } newTaskMetadata)
    {
        Console.WriteLine("Failed to create the task table: ");
        foreach (var message in taskMetadataStellarDsResult.Messages)
        {
            Console.WriteLine(message.Code + ": " + message.Message);
        }

        return;
    }

    foreach (var property in typeof(TaskPropertyMapper).GetProperties())
    {
        var stellarDsType = property.GetCustomAttribute<StellarDsType>()?.Name ?? property.PropertyType.ToString();

        var fieldMetadataStellarDsResult = await schemaApiService.CreateField(newTaskMetadata.Id, property.Name, stellarDsType);

        if (fieldMetadataStellarDsResult.Data is not null)
        {
            continue;
        }

        Console.WriteLine("Failed to create field for the task table: ");
        foreach (var message in fieldMetadataStellarDsResult.Messages)
        {
            Console.WriteLine(message.Code + ": " + message.Message);
        }

        return;
    }


    FileHelpers.WriteAppSettings(newListMetadata.Id, newTaskMetadata.Id, port, clientGuid, clientSecret, projectGuid, readOnlyJsonWebAccessToken);
});

// Open the system's default browser to the OAuth URL
var oauthUrl = $"https://stellards.io/oauth?client_id={clientGuid}&redirect_uri=https://localhost:{port}/oauth/oauthcallback&response_type=code";
Process.Start(new ProcessStartInfo
{
    FileName = oauthUrl,
    UseShellExecute = true
});

app.Run();