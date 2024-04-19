using System.Text.Json;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class StellarDsSettingsExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

        internal static async Task<StellarDsCredentials> CreateJsonFile(this StellarDsCredentials stellarDsCredentials)
        {
            var jsonString = JsonSerializer.Serialize(stellarDsCredentials, JsonSerializerOptions);

            await File.WriteAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.StellarDs.json"), jsonString);
            //todo: const for pathstring

            return stellarDsCredentials;
        }
    }
}
