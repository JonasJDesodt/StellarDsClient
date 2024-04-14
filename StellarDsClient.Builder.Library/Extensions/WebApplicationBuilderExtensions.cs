using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class WebApplicationBuilderExtensions
    {
        internal static WebApplicationBuilder AddServices(this WebApplicationBuilder builder, ApiSettings apiSettings, OAuthSettings oAuthSettings)
        {
            builder.Services.AddScoped<OAuthApiService>();


            builder.Services.AddSingleton(apiSettings);
            builder.Services.AddSingleton(oAuthSettings);

            builder.Services.AddHttpClient(apiSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
            });
            
            return builder;
        }
    }
}