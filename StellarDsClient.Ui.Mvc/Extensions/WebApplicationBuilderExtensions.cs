using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Models;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Stores;
using System.Diagnostics;
using StellarDsClient.Builder.Library.Models;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class WebApplicationBuilderExtensions 
    {
        internal static WebApplicationBuilder AddStellarDsClientServices(this WebApplicationBuilder builder, StellarDsSettings stellarDsSettings)
        {
            var apiSettings = stellarDsSettings.ApiSettings;
            builder.Services.AddSingleton(apiSettings);

            var oAuthSettings = stellarDsSettings.OAuthSettings;
            builder.Services.AddSingleton(oAuthSettings);

            var cookieSettings = builder.Configuration.GetSection(nameof(CookieSettings)).Get<CookieSettings>() ?? throw new NullReferenceException($"Unable to get the {nameof(CookieSettings)}");

            builder.Services.AddSingleton(cookieSettings);

            builder.Services.AddSingleton(stellarDsSettings.TableSettings);


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

            builder.Services.AddScoped<OAuthTokenProvider>();
            builder.Services.AddScoped<ReadonlyAccessTokenProvider>();
            builder.Services.AddScoped<DataApiService<OAuthTokenProvider>>();
            builder.Services.AddScoped<DataApiService<ReadonlyAccessTokenProvider>>();

            return builder;
        }
    }
}
