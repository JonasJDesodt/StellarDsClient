using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Extensions;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Sdk.Dto.Transfer;

namespace StellarDsClient.Sdk
{
    public class OAuthApiService(IHttpClientFactory httpClientFactory, ApiSettings apiSettings, OAuthCredentials oAuthCredentials, OAuthSettings oAuthSettings)
    {
        public string ClientId => oAuthCredentials.ClientId;
        public string RedirectUri => oAuthCredentials.RedirectUri;
        public string OAuthBaseAddress => oAuthSettings.BaseAddress;
        

        private readonly string _httpClientName = apiSettings.Name;

        private readonly string _requestUri = $"/{apiSettings.Version}/oauth/token";

        private readonly Dictionary<string, string> _clientParams = new()
        {
            { "client_id", oAuthCredentials.ClientId },
            { "client_secret", oAuthCredentials.ClientSecret }
        };


        public async Task<OAuthTokens> GetTokensAsync(string authorizationCode)
        {
            var httpResponse = await httpClientFactory
                .CreateClient(_httpClientName)
                .PostAsync(_requestUri, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", authorizationCode },
                    { "redirect_uri", oAuthCredentials.RedirectUri},
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

            var flag = await httpResponse.Content.ReadAsStringAsync();

            return await httpResponse.ToOAuthTokens();
        }
    }
}