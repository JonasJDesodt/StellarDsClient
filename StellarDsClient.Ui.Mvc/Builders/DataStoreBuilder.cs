using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Helpers;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Stores;

namespace StellarDsClient.Ui.Mvc.Builders
{
    internal static class DataStoreBuilder
    {
        internal static void CreateTables()
        {
            if (new ConfigurationBuilder().AddJsonFile("appsettings.StellarDsCredentials.json", true).Build().Get<StellarDsCredentials>() is not null) return;

            #region UserInput
            Console.WriteLine("Some of the credentials have special requirements. ");
            Console.WriteLine("You can find those requirements in the Readme."); //todo: folder path & github link
            Console.WriteLine("");

            Console.Write("ClientId: ");
            var clientId = Console.ReadLine();
            if (clientId is null) { return; }

            Console.Write("ClientSecret: ");
            var clientSecret = Console.ReadLine();
            if (clientSecret is null) { return; }

            Console.Write("ProjectId: ");
            var projectId = Console.ReadLine();
            if (projectId is null) { return; }

            Console.Write("ReadOnlyToken: ");
            var readOnlyToken = Console.ReadLine();
            if (readOnlyToken is null) { return; }

            Console.Write("Unique name for the 'list' table: ");
            var listTableName = Console.ReadLine();
            if (listTableName is null) { return; }

            Console.Write("Unique name for the 'toDo' table: ");
            var toDoTableName = Console.ReadLine();
            if (toDoTableName is null) { return; }
            #endregion

            var oAuthCredentials = new OAuthCredentials
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                RedirectUri = $"{EnvironmentHelpers.GetApplicationUrl()}/oauth/oauthcallback",
            };

            var apiCredentials = new ApiCredentials
            {
                Project = projectId,
                ReadOnlyToken = readOnlyToken
            };

            var models = new Dictionary<Type, string> { { typeof(List), listTableName }, { typeof(ToDo), "toDo_" + toDoTableName } };

            var minimalWebApplicationBuilder = WebApplication.CreateBuilder();

            var apiSettings = minimalWebApplicationBuilder.Configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>() ?? throw new NullReferenceException("Unable to get the ApiSettings from appsettings.json.");

            minimalWebApplicationBuilder.Services.AddHttpClient(apiSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
            });

            minimalWebApplicationBuilder.Services.AddHttpContextAccessor();
            minimalWebApplicationBuilder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ReturnUrlParameter = "returnUrl";
                    options.LoginPath = new PathString("/oauth");
                    options.SlidingExpiration = false;
                });

            var cookieSettings = minimalWebApplicationBuilder.Configuration.GetSection(nameof(CookieSettings)).Get<CookieSettings>() ?? throw new NullReferenceException($"Unable to get the {nameof(CookieSettings)}");

            var minimalWebApplication = minimalWebApplicationBuilder.Build();

            minimalWebApplication.MapGet("/", () => Results.Redirect($"https://stellards.io/oauth?client_id={oAuthCredentials.ClientId}&redirect_uri={oAuthCredentials.RedirectUri}&response_type=code"));

            minimalWebApplication.MapGet(
                "/oauth/oauthcallback", 
                async ([FromQuery] string code,
                    IHttpClientFactory httpClientFactory, 
                    IHttpContextAccessor httpContextAccessor,
                    HttpContext httpContext) =>
            {
                var oAuthApiService = new OAuthApiService(httpClientFactory, apiSettings, oAuthCredentials);

                var tokens = await oAuthApiService.GetTokensAsync(code);

                var oAuthTokenProvider = new OAuthTokenProvider(new OAuthTokenStore(httpContextAccessor, cookieSettings), oAuthApiService, httpContextAccessor);
                await oAuthTokenProvider.BrowserSignIn(tokens);

                var schemaService = new SchemaApiService<OAuthTokenProvider>(httpClientFactory, apiSettings, apiCredentials, oAuthTokenProvider);

                var tableSettings = new TableSettings { };
                foreach (var model in models)
                {
                    var stellarDsResult = await schemaService.CreateTable(model.Key, model.Value);

                    var metaData = 
                        stellarDsResult.Data ?? 
                        throw new NullReferenceException($"Unable to retrieve metadata for model {model.Key}. The table is probably not build.");
                    
                    tableSettings.Add(model.Key.Name, metaData.Id);
                }

                _ = new StellarDsCredentials
                {
                    TableSettings = tableSettings,
                    ApiCredentials = apiCredentials,
                    OAuthCredentials = oAuthCredentials
                }.CreateJsonFile();

                await httpContext.Response.WriteAsync("Please refresh the page to start the application");

                minimalWebApplication.Lifetime.StopApplication();
            });

            minimalWebApplication.Run();
        }
    }
}