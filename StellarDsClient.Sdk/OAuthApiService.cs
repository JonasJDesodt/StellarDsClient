using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Extensions;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Sdk
{
    public class OAuthApiService(IHttpClientFactory httpClientFactory, ApiSettings apiSettings, OAuthSettings oAuthSettings)
    {
        private readonly string _httpClientName = apiSettings.Name;

        private readonly string _requestUri = $"/{apiSettings.Version}/oauth/token";

        private readonly Dictionary<string, string> _clientParams = new()
        {
            { "client_id", oAuthSettings.ClientId },
            { "client_secret", oAuthSettings.ClientSecret }
        };

        public async Task<OAuthTokens> GetTokensAsync(string authorizationCode)
        {
            var httpResponse = await httpClientFactory
                .CreateClient(_httpClientName)
                .PostAsync(_requestUri, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", authorizationCode },
                    { "redirect_uri", oAuthSettings.RedirectUri},
                }.Concat(_clientParams)));

            return await httpResponse.ToOAuthTokens();
        }

        public async Task<OAuthTokens> PostRefreshTokenAsync(string refreshToken)
        {
            var httpResponse = await httpClientFactory
                .CreateClient(_httpClientName)
                .PostAsync(_requestUri, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken },
                }.Concat(_clientParams)));

            return await httpResponse.ToOAuthTokens();
        }
    }
}