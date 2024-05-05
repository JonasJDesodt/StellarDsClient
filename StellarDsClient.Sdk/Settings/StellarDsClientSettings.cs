namespace StellarDsClient.Sdk.Settings
{
    public class StellarDsClientSettings
    {
        public required ApiSettings ApiSettings { get; set; }

        public required OAuthSettings OAuthSettings { get; set; }
    }
}