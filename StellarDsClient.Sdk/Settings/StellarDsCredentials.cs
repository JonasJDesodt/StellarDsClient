namespace StellarDsClient.Sdk.Settings
{
    public class StellarDsCredentials
    {
        public required ApiCredentials ApiCredentials { get; set; }

        public required OAuthCredentials OAuthCredentials { get; set; }

        public required TableSettings TableSettings { get; set; }
    }
}