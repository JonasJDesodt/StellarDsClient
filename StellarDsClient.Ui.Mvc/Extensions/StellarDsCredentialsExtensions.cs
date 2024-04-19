using StellarDsClient.Sdk.Settings;
using System.Text.Json;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class StellarDsCredentialsExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
        internal static async Task<StellarDsCredentials> CreateJsonFile(this StellarDsCredentials stellarDsCredentials)
        {
            var jsonString = JsonSerializer.Serialize(stellarDsCredentials, JsonSerializerOptions);

            await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "appsettings.StellarDsCredentials.json"), jsonString);

            await File.WriteAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.StellarDsCredentials.json"), jsonString);
            //todo: const for pathstring

            return stellarDsCredentials;
        }
    }
}
