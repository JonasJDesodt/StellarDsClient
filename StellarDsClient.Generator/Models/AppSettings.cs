using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Models.Settings;

namespace StellarDsClient.Generator.Models
{
    internal class AppSettings
    {
        public required ApiSettings ApiSettings { get; set; }

        public required OAuthSettings OAuthSettings { get; set; }

        public required TableSettings TableSettings { get; set; }

        public required CookieSettings CookieSettings { get; set; }

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