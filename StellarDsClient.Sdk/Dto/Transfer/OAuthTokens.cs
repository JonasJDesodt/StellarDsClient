using System.Text.Json.Serialization;

namespace StellarDsClient.Sdk.Dto.Transfer
{
    public class OAuthTokens
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public required string RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public required long ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public required string TokenType { get; set; }
    }
}