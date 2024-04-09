using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Ui.Mvc.Providers
{
    public class ReadonlyAccessTokenProvider(ApiSettings apiSettings) : ITokenProvider
    {
        public Task<string> Get()
        {
            return Task.FromResult(apiSettings.ReadOnlyToken); //TODO: temp hack to get string?
        }
    }
}