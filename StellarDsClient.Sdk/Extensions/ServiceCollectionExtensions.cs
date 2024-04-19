using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace StellarDsClient.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStellarDsSettings(this IServiceCollection serviceCollection, StellarDsCredentials stellarDsCredentials)
        {
            serviceCollection.AddSingleton(stellarDsCredentials.ApiCredentials);

            serviceCollection.AddSingleton(stellarDsCredentials.OAuthCredentials);

            serviceCollection.AddSingleton(stellarDsCredentials.TableSettings);

            return serviceCollection;
        }

        public static IServiceCollection AddStellarDsHttpClients(this IServiceCollection serviceCollection, ApiSettings apiSettings, OAuthSettings oAuthSettings)
        {
            serviceCollection.AddHttpClient(apiSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
            });

            serviceCollection.AddHttpClient(oAuthSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(oAuthSettings.BaseAddress);
            });

            return serviceCollection;
        }
    }
}