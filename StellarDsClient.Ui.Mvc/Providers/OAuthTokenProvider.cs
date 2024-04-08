using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Ui.Mvc.Services;

namespace StellarDsClient.Ui.Mvc.Providers
{
    public class OAuthTokenProvider(IOAuthTokenStore iOAuthTokenStore, OAuthApiService oAuthApiService) : ITokenProvider
    {
        public async Task<string> Get()
        {
            var accessToken = iOAuthTokenStore.GetAccessToken();

            if (accessToken is not null && OAuthApiService.ValidateAccessToken(accessToken))
            {
                return accessToken;
            }

            //var tokens = await oAuthApiService.PostRefreshTokenAsync(refreshToken);

            //await oAuthClientService.BrowserSignIn(tokens);

            //return tokens.AccessToken;


            return await oAuthApiService.UseRefreshToken(iOAuthTokenStore.GetRefreshToken()); 
        }
    }
}