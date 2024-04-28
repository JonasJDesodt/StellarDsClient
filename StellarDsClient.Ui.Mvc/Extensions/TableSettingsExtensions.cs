using StellarDsClient.Models.Mappers;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class TableSettingsExtensions
    {
        internal static bool Validate(this TableSettings tableSettings)
        {
            if (!tableSettings.TryGetValue(nameof(List), out int listId))
            {
                return false;
            }

            if (listId <= 0)
            {
                return false;
            }

            if (!tableSettings.TryGetValue(nameof(ToDo), out int toDoId))
            {
                return false;
            }

            if (toDoId <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
