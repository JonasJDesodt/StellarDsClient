using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Ui.Mvc.Providers
{
    public class ReadonlyAccessTokenProvider(ApiCredentials apiCredentials) : ITokenProvider
    {
        public Task<string> Get()
        {
            return Task.FromResult(apiCredentials.ReadOnlyToken); //TODO: temp hack to get string?
        }
    }
}