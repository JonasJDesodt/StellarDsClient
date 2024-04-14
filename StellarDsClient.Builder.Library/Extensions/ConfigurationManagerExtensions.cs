using StellarDsClient.Builder.Library.Helpers;
using StellarDsClient.Builder.Library.Models;
using StellarDsClient.Sdk.Models;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class ConfigurationManagerExtensions
    {
        internal static OAuthSettings GetOAuthSettings(this ConfigurationManager configurationManager, string applicationUrl)
        {
            return configurationManager.GetSection(nameof(OAuthSettings)).Get<OAuthSettings>() ?? AppSettingsHelpers.RequestOAuthSettings(applicationUrl);
        }

        internal static ApiSettings GetApiSettings(this ConfigurationManager configurationManager)
        {
            return configurationManager.GetSection(nameof(ApiSettings)).Get<ApiSettings>() ?? AppSettingsHelpers.RequestApiSettings();
        }

        internal static TableSettingsDictionary? GetTableSettings(this ConfigurationManager configurationManager)
        {
            return configurationManager.GetSection(nameof(TableSettings)).Get<TableSettingsDictionary>();
        }
    }
}