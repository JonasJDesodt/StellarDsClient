namespace StellarDsClient.Sdk.Settings
{
    public class StellarDsSettings
    {
        public required ApiSettings ApiSettings { get; set; }

        public required OAuthSettings OAuthSettings { get; set; }

        public required TableSettings TableSettings { get; set; }

        public bool? ValidateDatabaseOnLaunch { get; set; } = true;
    }
}