using System.Text.Json.Serialization;

namespace StellarDsClient.Ui.Mvc.Models.Settings
{
    public class LogLevel
    {
        public required string Default { get; set; } = "Information";

        [JsonPropertyName("Microsoft_AspNetCore")]
        public required string MicrosoftAspNetCore { get; set; } = "Warning";
    }
}
