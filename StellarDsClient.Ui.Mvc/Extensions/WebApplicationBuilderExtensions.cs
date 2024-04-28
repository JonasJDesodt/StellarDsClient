using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Stores;
using System.Diagnostics;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class WebApplicationBuilderExtensions 
    {
        internal static WebApplicationBuilder AddStellarDsClientServices(this WebApplicationBuilder webApplicationBuilder, TableSettings? tableSettings) 
        {
            tableSettings ??= [];



            return webApplicationBuilder;
        }


        //todo: move to sdk? 
        internal static WebApplicationBuilder AddStellarDsClientServices(this WebApplicationBuilder builder)
        {
            var stellarDsCredentials = new ConfigurationBuilder().AddJsonFile("appsettings.StellarDsCredentials.json", false).Build().Get<StellarDsCredentials>() ??
                                              throw new NullReferenceException("Unable to get the StellarDsCredentials from appsettings.StellarDsCredentials.json");
            
            var apiCredentials = stellarDsCredentials.ApiCredentials;
            builder.Services.AddSingleton(apiCredentials);

            var oAuthCredentials = stellarDsCredentials.OAuthCredentials;
            builder.Services.AddSingleton(oAuthCredentials);

            var cookieSettings = builder.Configuration.GetSection(nameof(CookieSettings)).Get<CookieSettings>() ?? throw new NullReferenceException($"Unable to get the {nameof(CookieSettings)}");
            builder.Services.AddSingleton(cookieSettings);

            builder.Services.AddSingleton(stellarDsCredentials.TableSettings);

            var apiSettings = builder.Configuration.GetSection(nameof(ApiSettings)).Get<ApiSettings>() ?? throw new NullReferenceException("Unable to get ApiSettings from appsettings.json");
            builder.Services.AddSingleton(apiSettings);

            var oAuthSettings = builder.Configuration.GetSection(nameof(OAuthSettings)).Get<OAuthSettings>() ?? throw new NullReferenceException("Unable to get OAuthSettings from appsettings.json");
            builder.Services.AddSingleton(oAuthSettings);

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

            return builder;
        }
    }
}
