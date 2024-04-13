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
using StellarDsClient.Sdk.Models;
using StellarDsClient.Sdk.Settings;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace StellarDsClient.Builder.Library
{
    public class DbBuilder
    {
        //todo: sync?
        public async Task<StellarDsSettings> Run(string[] args)
        {
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";");
            var applicationUrl = urls?.Single(x => x.StartsWith("https://"));
            if (string.IsNullOrWhiteSpace(applicationUrl))
            {
                throw new NullReferenceException("Unable to retrieve the application url from launchsettings.json");
            }


            //todo: test the localhostport?

            var builder = WebApplication.CreateBuilder(args); //todo: without args?

            var configuration = builder.Configuration;
            configuration.AddJsonFile("appsettings.StellarDs.json", true);

            // todo check if all the fields are valid / present
            var oAuthSettings = builder.Configuration.GetSection(nameof(OAuthSettings)).Get<OAuthSettings>() ?? AppSettingsHelpers.RequestOAuthSettings(applicationUrl);

            // todo check if all the fields are valid / present
            var apiSettings = builder.Configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>() ?? AppSettingsHelpers.RequestApiSettings();

            // todo check if all the fields are valid / present
            var tableSettings = builder.Configuration.GetSection(nameof(TableSettings)).Get<TableSettingsDictionary>();
            if (tableSettings is not null)
            {
                return new StellarDsSettings
                {
                    ApiSettings = apiSettings,
                    OAuthSettings = oAuthSettings,
                    TableSettings = tableSettings
                };
            }
    
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

            var app = builder.Build();
            
            app.MapGet("/", context =>
            {
                context.Response.Redirect($"https://stellards.io/oauth?client_id={oAuthSettings.ClientId}&redirect_uri={oAuthSettings.RedirectUri}&response_type=code");

                return Task.CompletedTask;
            });

            //todo: what happens on 'return'? => throw exceptions?
            app.MapGet("/oauth/oauthcallback", async context =>
            {
                if (context.RequestServices.GetService<OAuthApiService>() is not { } oAuthApiService)
                {
                    throw new NullReferenceException($"Unable to get the {nameof(OAuthApiService)}");
                }

                var code = context.Request.Query["code"].ToString();
                var state = context.Request.Query["state"]; // todo?!

                if (string.IsNullOrWhiteSpace(code.ToString()))
                {
                    throw new NullReferenceException($"Unable to get the authorization code");
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
                    new JsonWebTokenHandler().ReadJsonWebToken(tokens.AccessToken);
                }
                catch (Exception exception)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync($"The access token is not a valid JsonWebToken {exception.Message}. You can close the browser.");

                    return;
                }

                if (context.RequestServices.GetService<AccessTokenProvider>() is not { } accessTokenProvider)
                {
                    throw new NullReferenceException($"Unable to get the {nameof(AccessTokenProvider)}");
                }

                accessTokenProvider.Set(accessToken);

                if (context.RequestServices.GetService<SchemaApiService<AccessTokenProvider>>() is not { } schemaApiService)
                {
                    throw new NullReferenceException($"Unable to get the {nameof(SchemaApiService<AccessTokenProvider>)}");
                }

                tableSettings = await schemaApiService.BuildDatabase();

                await context.Response.WriteAsync("Refresh the page to open the StellarDsClient web app.");

                //app.Lifetime.StopApplication();
                var lifetime = context.RequestServices.GetRequiredService<IHostApplicationLifetime>();

                lifetime.StopApplication();
                //todo: dispose app, resources, services????
                //todo => test => await app.DisposeAsync();
            });

            
            await app.RunAsync();


            //todo => test => await app.DisposeAsync();


            return await new StellarDsSettings
            {
                ApiSettings = apiSettings,
                OAuthSettings = oAuthSettings,
                TableSettings = tableSettings ?? throw new NullReferenceException("Unable to create the StellarDsSettings. TableSettings is null")
            }.CreateJsonFile();
        }
    }
}