using StellarDsClient.Sdk.Models;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class TableSettingsDictionaryExtensions
    {
        internal static bool Validate(this TableSettingsDictionary tableSettingsDictionary, List<Type> models)
        {
            var keys = tableSettingsDictionary.Select(x => x.Key).ToList();

            return models.All(model => keys.SingleOrDefault(x => x == model.Name) is not null);
        }
    }
}