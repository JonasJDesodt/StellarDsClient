using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Stores;
using System.Diagnostics;
using System.Reflection;

namespace StellarDsClient.Ui.Mvc.Builders
{
    public static class DatabaseBuilder
    {
        public static TableSettings? CreateTables(CookieSettings cookieSettings, OAuthSettings oAuthSettings, ApiSettings apiSettings, OAuthCredentials oAuthCredentials, ApiCredentials apiCredentials)
        {
            var builder = WebApplication.CreateBuilder();

            TableSettings? tableSettings = null;

            builder.Services.AddHttpClient(apiSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ReturnUrlParameter = "returnUrl";
                    options.LoginPath = new PathString("/oauth");
                    options.SlidingExpiration = false;
                });

            var minimalWebApplication = builder.Build();

            minimalWebApplication.UseExceptionHandler("/error");

            minimalWebApplication.MapGet("/error", async (HttpContext httpContext) =>
            {
                //todo: write & log the error
                await httpContext.Response.WriteAsync($"Errortje!");

                minimalWebApplication.Lifetime.StopApplication();
            });

            minimalWebApplication.MapGet("/", () => Results.Redirect($"https://stellards.io/oauth?client_id={oAuthCredentials.ClientId}&redirect_uri={oAuthCredentials.RedirectUri}&response_type=code"));

            minimalWebApplication.MapGet("/oauth/oauthcallback", async ([FromQuery] string code, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, HttpContext httpContext) =>
            {
                var oAuthApiService = new OAuthApiService(httpClientFactory, apiSettings, oAuthCredentials, oAuthSettings);

                var tokens = await oAuthApiService.GetTokensAsync(code);

                var oAuthAccessTokenProvider = new OAuthAccessTokenProvider(new OAuthTokenStore(httpContextAccessor, cookieSettings), oAuthApiService, httpContextAccessor);
                await oAuthAccessTokenProvider.BrowserSignIn(tokens);

                var schemaService = new SchemaApiService<OAuthAccessTokenProvider>(httpClientFactory, apiSettings, apiCredentials, oAuthAccessTokenProvider);

                var tableSettings = new TableSettings { };

                var tablesStellarDsResult = await schemaService.FindTables();

                if (tablesStellarDsResult.IsSuccess is false || tablesStellarDsResult.Data is not { } tables)
                {
                    return Results.Problem(title: "Unable to return the tables metadata.", detail: string.Join(Environment.NewLine, tablesStellarDsResult.Messages), statusCode: 500);
                }

                tables = [.. tables.OrderByDescending(t => t.Name.Length)];

                var toDoTableName = nameof(ToDo);
                if (tables.FirstOrDefault(t => t.Name.StartsWith(toDoTableName, StringComparison.InvariantCultureIgnoreCase))?.Name is { } existingToDoTableName)
                {
                    toDoTableName = existingToDoTableName + "_1";
                }

                var listTableName = nameof(List);
                if (tables.FirstOrDefault(t => t.Name.StartsWith(listTableName, StringComparison.InvariantCultureIgnoreCase))?.Name is { } existingListTableName)
                {
                    listTableName = existingListTableName + "_1";
                }

                var toDoTableStellarDsResult = await schemaService.CreateTable(typeof(ToDo), toDoTableName);
                if (toDoTableStellarDsResult.IsSuccess is false || toDoTableStellarDsResult.Data is not { } toDoTableMetaData)
                {
  
                    return Results.Problem(title: $"Unable to create the {nameof(ToDo)} table", detail: string.Join(Environment.NewLine, toDoTableStellarDsResult.Messages), statusCode: 500);
                }

                var listTableStellarDsResult = await schemaService.CreateTable(typeof(List), listTableName);
                if (listTableStellarDsResult.IsSuccess is false || listTableStellarDsResult.Data is not { } listTableMetaData)
                {
                    await schemaService.DeleteTable(toDoTableMetaData.Id);

                    return Results.Problem(title: $"Unable to create the {nameof(List)} table", detail: string.Join(Environment.NewLine, toDoTableStellarDsResult.Messages), statusCode: 500);
                }

                tableSettings = new TableSettings { { nameof(List), listTableMetaData.Id }, { nameof(ToDo), toDoTableMetaData.Id } };

                //await httpContext.Response.WriteAsync();

                //minimalWebApplication.Lifetime.StopApplication();

                return Results.Ok($"Please add the following parameters as integers to the TableSettings section in the appsettings.json file: \n\tList: {listTableMetaData.Id}, ToDo: {toDoTableMetaData.Id}. \nOmitting these parameters from the appsettings.json file will cause the application to build the tables again on the next launch in debug mode.\n\r\tRefreshing the page will start the application.");
            });

            minimalWebApplication.Run();

            return tableSettings;
        }
    }
}