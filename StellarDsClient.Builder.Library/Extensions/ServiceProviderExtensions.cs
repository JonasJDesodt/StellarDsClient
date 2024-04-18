using StellarDsClient.Builder.Library.Providers;
using StellarDsClient.Sdk;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class ServiceProviderExtensions
    {
        //todo: use generic extension to get the services from the serviceprovider & throw exceptions if necessary

        internal static ServiceProvider SetAccessToken(this ServiceProvider serviceProvider, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) //todo: check if valid jwt token?
            {
                throw new NullReferenceException("There is no access token provided");
            }


            if (serviceProvider.GetService<AccessTokenProvider>() is not { } accessTokenProvider)
            {
                throw new NullReferenceException($"Unable to get the {nameof(AccessTokenProvider)}");
            }
            accessTokenProvider.Set(accessToken);

            return serviceProvider;
        }

        internal static SchemaApiService<AccessTokenProvider> GetSchemaApiService(this ServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService<SchemaApiService<AccessTokenProvider>>() is not { } schemaApiService)
            {
                throw new NullReferenceException($"Unable to get the {nameof(SchemaApiService<AccessTokenProvider>)}");
            }

            return schemaApiService;
        }
    }
}
