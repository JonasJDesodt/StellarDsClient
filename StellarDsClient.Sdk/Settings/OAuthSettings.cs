namespace StellarDsClient.Sdk.Settings
{
    public class OAuthSettings
    {
        public required string Name { get; set; }

        public required string BaseAddress { get; set; }

        public required string ClientId { get; set; }

        public required string ClientSecret { get; set; }

        public required string RedirectUri { get; set; }
    }
}