using StellarDsClient.Sdk.Abstractions;

namespace StellarDsClient.Ui.Blazor.Providers
{
    public class ReadOnlyAccessTokenProvider(string readOnlyAccessToken) : ITokenProvider
    {
        public async Task<string> Get()
        {
            return readOnlyAccessToken;
        }
    }
}