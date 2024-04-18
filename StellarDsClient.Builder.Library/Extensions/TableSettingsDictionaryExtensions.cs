using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class TableSettingsDictionaryExtensions
    {
        internal static bool Validate(this TableSettings tableSettings, List<Type> models)
        {
            //todo: return false if there is a duplicate or e.g. list & List ? 

            var keys = tableSettings.Select(x => x.Key).ToList();

            return models.All(model => keys.SingleOrDefault(x => x == model.Name) is not null);
        }
    }
}