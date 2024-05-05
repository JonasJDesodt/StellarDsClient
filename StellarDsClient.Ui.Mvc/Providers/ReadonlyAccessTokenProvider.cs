using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Ui.Mvc.Providers
{
    public class ReadonlyAccessTokenProvider(StellarDsClientSettings stellarDsClientSettings) : ITokenProvider
    {
        public Task<string> Get()
        {
            return Task.FromResult(stellarDsClientSettings.ApiSettings.ReadOnlyToken); //TODO: temp hack to get string?
        }
    }
}