using System.Text.Json.Serialization;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Models
{
    internal class AppSettings
    {
        public required ApiSettings ApiSettings { get; set; }

        public required OAuthSettings OAuthSettings { get; set; }

        public required TableSettings TableSettings { get; set; }

        //public required CookieSettings CookieSettings { get; set; }

        public required Logging Logging { get; set; }

        public required string AllowedHosts { get; set; }
    }

    internal class Logging
    {
        public required LogLevel LogLevel { get; set; }
    }

    internal class LogLevel
    {
        public required string Default { get; set; }

        [JsonPropertyName("Microsoft.AspNetCore")]
        public required string MicrosoftAspNetCore { get; set; }
    }
}