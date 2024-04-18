namespace StellarDsClient.Sdk.Settings
{
    public class ApiSettings
    {
        public required string Name { get; set; }

        public required string BaseAddress { get; set; }

        public required string Version { get; set; }

        public required string Project { get; set; }

        public required string ReadOnlyToken { get; set; }
    }
}