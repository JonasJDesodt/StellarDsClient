using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Builder.Library.Extensions;
using StellarDsClient.Builder.Library.Helpers;
using StellarDsClient.Builder.Library.Providers;
using StellarDsClient.Sdk;
using System.Reflection;
using StellarDsClient.Builder.Library.Models;
using StellarDsClient.Sdk.Models;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Builder.Library.Attributes;

namespace StellarDsClient.Builder.Library
{
    public class DbBuilder
    {
        //todo: sync?
        public async Task<StellarDsSettings> Run(string[] args, List<Type> models)
        {
            //todo: create record with the settings + the model?
            models.ForEach(m =>
            {
                if (m.GetCustomAttribute<StellarDsTable>() is null)
                {
                    throw new NullReferenceException($"The {m.Name} model is not annotated with the {nameof(StellarDsTable)} attribute");
                }
            });

            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";");
            var applicationUrl = urls?.Single(x => x.StartsWith("https://"));
            if (string.IsNullOrWhiteSpace(applicationUrl))
            {
                throw new NullReferenceException("Unable to retrieve the application url from launchsettings.json");
            }

            //todo: test the localhostport?

            var builder = WebApplication.CreateBuilder(args); //todo: without args?

            builder.Configuration.AddJsonFile("appsettings.StellarDs.json", true);

            var oAuthSettings = builder.Configuration.GetSection(nameof(OAuthSettings)).Get<OAuthSettings>() ?? AppSettingsHelpers.RequestOAuthSettings(applicationUrl);

            var apiSettings = builder.Configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>() ?? AppSettingsHelpers.RequestApiSettings();

            var tableSettings = builder.Configuration.GetSection(nameof(TableSettings)).Get<TableSettingsDictionary>();
            if (tableSettings is not null && tableSettings.Validate(models))
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

            //builder.Services.AddSingleton(new AccessTokenProvider());
            //builder.Services.AddScoped<SchemaApiService<AccessTokenProvider>>();

            builder.Services.AddSingleton(apiSettings);
            builder.Services.AddSingleton(oAuthSettings);

            builder.Services.AddHttpClient(apiSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
            });

            var app = builder.Build();

            var accessToken = string.Empty;

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

                if (tokens?.AccessToken is not { } token)
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

                accessToken = token;

                await context.Response.WriteAsync("Please continue in the console. Do not close this browser tab. You will need it later.");

                app.Lifetime.StopApplication();
            });


            await app.RunAsync();

            await app.DisposeAsync(); //todo: check if necessary, ServiceProvider is disposed on Lifetime.StopApplication()

            
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new NullReferenceException("There is no access token provided");
            }



            var serviceProvider = new ServiceCollection();

            serviceProvider.AddHttpClient(apiSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
            });

            serviceProvider.AddSingleton(new AccessTokenProvider());
            serviceProvider.AddScoped<SchemaApiService<AccessTokenProvider>>();
            serviceProvider.AddSingleton(apiSettings);





            var services = serviceProvider.BuildServiceProvider();

            if (services.GetService<AccessTokenProvider>() is not { } accessTokenProvider)
            {
                throw new NullReferenceException($"Unable to get the {nameof(AccessTokenProvider)}");
            }
            accessTokenProvider.Set(accessToken);

            if (services.GetService<SchemaApiService<AccessTokenProvider>>() is not { } schemaApiService)
            {
                throw new NullReferenceException($"Unable to get the {nameof(SchemaApiService<AccessTokenProvider>)}");
            }

            tableSettings = await schemaApiService.BuildDatabase(models);

            return await new StellarDsSettings
            {
                ApiSettings = apiSettings,
                OAuthSettings = oAuthSettings,
                TableSettings = tableSettings ?? throw new NullReferenceException("Unable to create the StellarDsSettings. TableSettings is null")
            }.CreateJsonFile();
        }
    }
}