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
        public static IServiceCollection AddStellarDsSettings(this IServiceCollection serviceCollection, StellarDsSettings stellarDsSettings)
        {
            serviceCollection.AddSingleton(stellarDsSettings.ApiSettings);

            serviceCollection.AddSingleton(stellarDsSettings.OAuthSettings);

            serviceCollection.AddSingleton(stellarDsSettings.TableSettings);
            
            return serviceCollection;
        }

        public static IServiceCollection AddHttpClients(this IServiceCollection serviceCollection, StellarDsSettings stellarDsSettings)
        {
            var apiSettings = stellarDsSettings.ApiSettings;
            serviceCollection.AddHttpClient(apiSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
            });

            var oAuthSettings = stellarDsSettings.OAuthSettings;
            serviceCollection.AddHttpClient(oAuthSettings.Name, httpClient =>
            {
                httpClient.BaseAddress = new Uri(oAuthSettings.BaseAddress);
            });

            return serviceCollection;
        }
    }
}