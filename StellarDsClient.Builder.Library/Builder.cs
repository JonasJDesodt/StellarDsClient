using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Builder.Library.Extensions;
using StellarDsClient.Builder.Library.Helpers;
using StellarDsClient.Builder.Library.Providers;
using StellarDsClient.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarDsClient.Builder.Library.Models;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library
{
    public class Builder
    {
        //todo: sync?
        public async Task<StellarDsSettings> Run(string[] args)
        {
            //only works in debug? 
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";");

            if (!int.TryParse(urls?.Single(x => x.StartsWith("https://localhost:")).Split(':').Last(), out var localhostPort))
            {
                localhostPort = AppSettingsHelpers.RequestLocalhostPort();
            };
 
            //todo: test the localhostport?
            
            var jsonWebTokenHandler = new JsonWebTokenHandler();

            var builder = WebApplication.CreateBuilder(args); //todo: without args?
            
            // todo check if all the fields are valid / present
            var oAuthSettings = builder.Configuration.GetSection(nameof(OAuthSettings)).Get<OAuthSettings>() ?? AppSettingsHelpers.RequestOAuthSettings(localhostPort);

            // todo check if all the fields are valid / present
            var apiSettings = builder.Configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>() ?? AppSettingsHelpers.RequestApiSettings();

            // todo check if all the fields are valid / present
            //todo: use tablesettingsdictionary instead of tablesettings
            var tableSettings = builder.Configuration.GetSection(nameof(TableSettings)).Get<TableSettings>();

            if (tableSettings is not null)
            {
                return new StellarDsSettings
                {
                    ApiSettings = apiSettings,
                    OAuthSettings = oAuthSettings,
                    TableSettings = tableSettings
                };
            }
    
            //var kestrelConfig = builder.Configuration.GetSection("Kestrel:Endpoints:Http:Url").Value ?? throw new NullReferenceException("KestrelConfiguration is null");
            //var localhostPort = new Uri(kestrelConfig).Port;
         
            // Add services
            // TODO: dispose services?
            builder.Services.AddScoped<OAuthApiService>();

            builder.Services.AddSingleton(new AccessTokenProvider());
            builder.Services.AddScoped<SchemaApiService<AccessTokenProvider>>();

            builder.Services.AddSingleton(apiSettings);
            builder.Services.AddSingleton(oAuthSettings);

            builder.Services.AddHttpClient(apiSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
            });

           // TableSettings? tableSettings = null;

            var app = builder.Build();

            //todo: what happens on 'return'? => throw exceptions?
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

                    await context.Response.WriteAsync("The access token is a valid JsonWebToken. You can close the browser. Refresh the page to open the StellarDsClient website.");
                }
                catch (Exception exception)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync($"The access token is not a valid JsonWebToken {exception.Message}. You can close the browser.");

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

                tableSettings = await schemaApiService.BuildDatabase();

                var lifetime = context.RequestServices.GetRequiredService<IHostApplicationLifetime>();

                lifetime.StopApplication();
                //dispose app, resources, services????
            });

            var oauthUrl = $"https://stellards.io/oauth?client_id={oAuthSettings.ClientId}&redirect_uri={oAuthSettings.RedirectUri}&response_type=code";
            Process.Start(new ProcessStartInfo
            {
                FileName = oauthUrl,
                UseShellExecute = true
            });

            await app.RunAsync();

            return new StellarDsSettings
            {
                ApiSettings = apiSettings,
                OAuthSettings = oAuthSettings,
                TableSettings = tableSettings ?? throw new NullReferenceException("Unable to create the StellarDsSettings. TableSettings is null") };
        }
    }
}