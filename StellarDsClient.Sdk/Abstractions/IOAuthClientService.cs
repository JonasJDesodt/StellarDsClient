using StellarDsClient.Dto.Transfer;

namespace StellarDsClient.Sdk.Abstractions
{
    public interface IOAuthClientService
    {
        Task BrowserSignIn(OAuthTokens oAuthTokens);

        Task BrowserSignOut();
    }
}