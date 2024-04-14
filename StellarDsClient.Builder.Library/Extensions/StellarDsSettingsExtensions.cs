﻿using StellarDsClient.Builder.Library.Models;
using System.Text.Json;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class StellarDsSettingsExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

        internal static async Task<StellarDsSettings> CreateJsonFile(this StellarDsSettings stellarDsSettings)
        {
            var jsonString = JsonSerializer.Serialize(stellarDsSettings, JsonSerializerOptions);

            await File.WriteAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.StellarDs.json"), jsonString);

            return stellarDsSettings;
        }
    }
}