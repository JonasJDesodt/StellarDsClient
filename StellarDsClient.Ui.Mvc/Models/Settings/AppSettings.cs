using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Ui.Mvc.Models.Settings
{
    public class AppSettings
    {
        public required ApiSettings ApiSettings { get; set; }

        public required OAuthSettings OAuthSettings { get; set; }

        public required ApiCredentials ApiCredentials { get; set; }

        public required OAuthCredentials OAuthCredentials { get; set; }

        public required CookieSettings CookieSettings { get; set; }

        public required string AllowedHosts { get; set; } = "*";
    }
}
