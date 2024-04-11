using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Models
{
    public class StellarDsSettings
    {
        public required ApiSettings ApiSettings { get; set; }

        public required OAuthSettings OAuthSettings { get; set; }

        public required TableSettings TableSettings { get; set; }
    }
}
