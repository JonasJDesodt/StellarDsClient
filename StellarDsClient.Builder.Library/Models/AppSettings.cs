using System.Text.Json.Serialization;
using StellarDsClient.Sdk.Models;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Models
{
    internal class AppSettings
    {
        public required ApiSettings ApiSettings { get; set; }

        public required OAuthSettings OAuthSettings { get; set; }

        public required TableSettings TableSettings { get; set; }

        //todo: remove?
        public required Logging Logging { get; set; }

        //todo: remove?
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