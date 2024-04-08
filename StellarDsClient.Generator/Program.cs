using System.Net.Http.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Generator.Models;
using StellarDsClient.Sdk.Extensions;
using StellarDsClient.Generator.Attributes;
using StellarDsClient.Generator.Extensions;
using System.Reflection;
using System.Text.Json;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Models.Settings;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using StellarDsClient.Generator.Helpers;

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
    readOnlyJsonWebAccessToken = new JsonWebTokenHandler().ReadJsonWebToken(readOnlyAccessToken);
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


var app = builder.Build();



app.MapGet("/oauth/oauthcallback", async context =>
{
    var code = context.Request.Query["code"];
    var state = context.Request.Query["state"];

    // Your logic to handle the authorization code and exchange it for tokens

    var clientParams = new Dictionary<string, string>()
    {
        { "client_id", clientGuid.ToString() },
        { "client_secret", clientSecret },
        { "grant_type", "authorization_code" },
        { "code", code },
        { "redirect_uri", $"https://localhost:{port}/oauth/oauthcallback" },
    };

    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://api.stellards.io");
    var httpTokensResponseMessage = await httpClient.PostAsync("/v1/oauth/token", new FormUrlEncodedContent(clientParams));
    var tokens = await httpTokensResponseMessage.Content.ReadFromJsonAsync<OAuthTokens>();

    if (tokens?.AccessToken != null)
    {
        try
        {
            new JsonWebTokenHandler().ReadJsonWebToken(tokens.AccessToken);
            await context.Response.WriteAsync("Token is valid. You can close the browser.");

            httpClient.AddAuthorization(tokens.AccessToken);
        }
        catch (Exception exception)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync($"Invalid token: {exception.Message}");

            return;
        }
    }
    else
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Failed to obtain tokens.");

        return;
    }

    var tablesMetaDataHttpResponseMessage = await httpClient.GetAsync($"v1/schema/table?project={projectGuid}");
    tablesMetaDataHttpResponseMessage.EnsureSuccessStatusCode();

    if (await tablesMetaDataHttpResponseMessage.Content.ReadFromJsonAsync<StellarDsResult<IList<TableResult>>>() is not { } stellarDsTablesMetaDataResult)
    {
        Console.WriteLine("Unable to fetch the tables metadata");

        return;
    }

    if (stellarDsTablesMetaDataResult.Data is null)
    {
        Console.WriteLine("Unable to retrieve metadata");
        return;
    }

    if (stellarDsTablesMetaDataResult.Data.GetMetadata("list") is { } listMetadata)
    {
        (await httpClient.DeleteAsync($"v1/schema/table?project={projectGuid}&table={listMetadata.Id}")).EnsureSuccessStatusCode();
    }

    if (stellarDsTablesMetaDataResult.Data.GetMetadata("task") is { } taskMetadata)
    {
        (await httpClient.DeleteAsync($"v1/schema/table?project={projectGuid}&table={taskMetadata.Id}")).EnsureSuccessStatusCode();
    }

    var listMetadataResponseMessage = await httpClient.PostAsJsonAsync($"v1/schema/table?project={projectGuid}", new { name = "list", isMultitenant = true });
    listMetadataResponseMessage.EnsureSuccessStatusCode();



    if ((await listMetadataResponseMessage.Content.ReadFromJsonAsync<StellarDsResult<TableResult>>())?.Data is not { } listTableResult)
    {
        Console.WriteLine("ERROR");

        return;
    }

    foreach (var property in typeof(ListPropertyMapper).GetProperties())
    {
        var stellarDsType = property.GetCustomAttribute<StellarDsType>()?.Name ?? property.PropertyType.ToString();

        (await httpClient.PostAsJsonAsync($"v1/schema/table/field?project={projectGuid}&table={listTableResult.Id}", new { name = property.Name, type = stellarDsType })).EnsureSuccessStatusCode();
    }


    var taskMetadataResponseMessage = await httpClient.PostAsJsonAsync($"v1/schema/table?project={projectGuid}", new { name = "task", isMultitenant = true });
    taskMetadataResponseMessage.EnsureSuccessStatusCode();

    if ((await taskMetadataResponseMessage.Content.ReadFromJsonAsync<StellarDsResult<TableResult>>())?.Data is not { } taskTableResult)
    {
        Console.WriteLine("ERROR");

        return;
    }

    foreach (var property in typeof(TaskPropertyMapper).GetProperties())
    {
        var stellarDsType = property.GetCustomAttribute<StellarDsType>()?.Name ?? property.PropertyType.ToString();

        (await httpClient.PostAsJsonAsync($"v1/schema/table/field?project={projectGuid}&table={taskTableResult.Id}", new { name = property.Name, type = stellarDsType })).EnsureSuccessStatusCode();
    }

    FileHelpers.WriteAppSettings(listTableResult.Id, taskTableResult.Id, port, clientGuid, clientSecret, projectGuid, readOnlyJsonWebAccessToken);
});

// Open the system's default browser to the OAuth URL
var oauthUrl = $"https://stellards.io/oauth?client_id={clientGuid}&redirect_uri=https://localhost:{port}/oauth/oauthcallback&response_type=code";
Process.Start(new ProcessStartInfo
{
    FileName = oauthUrl,
    UseShellExecute = true
});

app.Run();