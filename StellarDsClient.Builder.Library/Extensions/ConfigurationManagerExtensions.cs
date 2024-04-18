using StellarDsClient.Builder.Library.Helpers;
using StellarDsClient.Sdk.Settings;
using TableSettings = StellarDsClient.Sdk.Settings.TableSettings;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class ConfigurationManagerExtensions
    {
        internal static OAuthSettings GetOAuthSettings(this ConfigurationManager configurationManager)
        {
            return configurationManager.GetSection(nameof(OAuthSettings)).Get<OAuthSettings>() ?? AppSettingsHelpers.RequestOAuthSettings(EnvironmentHelpers.GetApplicationUrl());
        }

        internal static ApiSettings GetApiSettings(this ConfigurationManager configurationManager)
        {
            return configurationManager.GetSection(nameof(ApiSettings)).Get<ApiSettings>() ?? AppSettingsHelpers.RequestApiSettings();
        }

        internal static TableSettings? GetTableSettings(this ConfigurationManager configurationManager)
        {
            return configurationManager.GetSection(nameof(TableSettings)).Get<TableSettings>();
        }
    }
}