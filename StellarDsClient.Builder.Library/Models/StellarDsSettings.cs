using StellarDsClient.Sdk.Models;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Models
{
    public class StellarDsSettings
    {
        public required ApiSettings ApiSettings { get; set; }

        public required OAuthSettings OAuthSettings { get; set; }

        public required TableSettingsDictionary TableSettings { get; set; }

        public bool? ValidateDatabaseOnLaunch { get; set; } = true;
    }
}
