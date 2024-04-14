using Microsoft.Extensions.DependencyInjection;
using StellarDsClient.Builder.Library.Providers;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static ServiceProvider GetDataStoreBuilderServiceProvider(this ServiceCollection serviceCollection, ApiSettings apiSettings)
        {
            return serviceCollection.AddDsBuilderServices(apiSettings).BuildServiceProvider();
        }
        internal static ServiceCollection AddDsBuilderServices(this ServiceCollection serviceCollection, ApiSettings apiSettings)
        {
            serviceCollection
                .AddSingleton(new AccessTokenProvider())
                .AddScoped<SchemaApiService<AccessTokenProvider>>()
                .AddSingleton(apiSettings)
                .AddHttpClient(apiSettings.Name, httpClient =>
                {
                    httpClient.BaseAddress = new Uri(apiSettings.BaseAddress);
                });

            return serviceCollection;
        }
    }
}
