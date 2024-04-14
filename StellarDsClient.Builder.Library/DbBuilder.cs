using System.Diagnostics.Contracts;
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
        public const string StellarDsSettingsPath = "appsettings.StellarDs.json";

        public async Task<StellarDsSettings> Run(List<Type> models)
        {
            models.EnsureStellarDsTableAnnotations();

            var builder = WebApplication.CreateBuilder();

            builder.Configuration.AddJsonFile(StellarDsSettingsPath, true);

            var apiSettings = builder.Configuration.GetApiSettings();
            var oAuthSettings = builder.Configuration.GetOAuthSettings();
            var tableSettings = builder.Configuration.GetTableSettings();

            if (tableSettings is not null && tableSettings.Validate(models))
            {
                return new StellarDsSettings
                {
                    ApiSettings = apiSettings,
                    OAuthSettings = oAuthSettings,
                    TableSettings = tableSettings
                };
            }

            builder.AddServices(apiSettings, oAuthSettings);

            var app = builder.Build();

            string accessToken = null!;

            app.MapGet("/", context =>
            {
                context.Response.Redirect($"https://stellards.io/oauth?client_id={oAuthSettings.ClientId}&redirect_uri={oAuthSettings.RedirectUri}&response_type=code");

                return Task.CompletedTask;
            });

            app.MapGet("/oauth/oauthcallback", async context =>
            {
                var oAuthApiService = context.GetOauthApiService();

                //note: the state parameter is not implemented by StellarDs
                
                accessToken = await oAuthApiService.GetAccessToken(context.GetAuthorizationCode());

                await context.Response.WriteAsync("Please continue in the console. Do not close this browser tab. You will need it later.");

                app.Lifetime.StopApplication();
            });


            await app.RunAsync();

            await app.DisposeAsync(); 


            var serviceProvider = new ServiceCollection()
                .GetDataStoreBuilderServiceProvider(apiSettings)
                .SetAccessToken(accessToken);

            tableSettings = await serviceProvider
                .GetSchemaApiService()
                .BuildDataStore(models);

            await serviceProvider.DisposeAsync();

            return await new StellarDsSettings
            {
                ApiSettings = apiSettings,
                OAuthSettings = oAuthSettings,
                TableSettings = tableSettings
            }.CreateJsonFile();
        }
    }
}