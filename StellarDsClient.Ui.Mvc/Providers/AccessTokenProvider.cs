using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Ui.Mvc.Providers
{
    public class AccessTokenProvider(ApiSettings apiSettings) : ITokenProvider
    {
        public Task<string> Get()
        {
            return Task.FromResult(apiSettings.ReadOnlyToken); //temp hack to get string?
        }

    }
}