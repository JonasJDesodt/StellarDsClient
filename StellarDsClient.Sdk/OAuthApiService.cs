using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Extensions;
using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Sdk
{
    public class OAuthApiService(IHttpClientFactory httpClientFactory, StellarDsClientSettings stellarDsClientSettings)
    {
        public string ClientId => stellarDsClientSettings.OAuthSettings.ClientId;
        public string RedirectUri => stellarDsClientSettings.OAuthSettings.RedirectUri;
        public string OAuthBaseAddress => stellarDsClientSettings.OAuthSettings.BaseAddress;
        

        private readonly string _httpClientName = stellarDsClientSettings.ApiSettings.Name;

        private readonly string _requestUri = $"/{stellarDsClientSettings.ApiSettings.Version}/oauth/token";

        private readonly Dictionary<string, string> _clientParams = new()
        {
            { "client_id", stellarDsClientSettings.OAuthSettings.ClientId },
            { "client_secret", stellarDsClientSettings.OAuthSettings.ClientSecret }
        };


        public async Task<OAuthTokens> GetTokensAsync(string authorizationCode)
        {
            var httpResponse = await httpClientFactory
                .CreateClient(_httpClientName)
                .PostAsync(_requestUri, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", authorizationCode },
                    { "redirect_uri", stellarDsClientSettings.OAuthSettings.RedirectUri},
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


    
   

    public class Car 
    {
        private string? _color;
        public string? Color 
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
            }
        
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Car batMobile = new Car();
            batMobile.Color = "Black";  

            string c = batMobile.Color;
        }
    }









}