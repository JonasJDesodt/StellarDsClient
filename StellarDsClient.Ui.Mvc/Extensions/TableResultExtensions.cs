using StellarDsClient.Sdk.Attributes;
using StellarDsClient.Sdk.Dto.Schema;
using System.Reflection;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class TableResultExtensions
    {
        internal static bool IsValid(this TableResult tableResult, IList<FieldResult> fieldResults, Type model)
        {
            var stellarDsTable = model.GetCustomAttribute<StellarDsTable>();

            if (stellarDsTable is null)
            {
                return false;
            }

            if (tableResult.IsMultitenant != stellarDsTable.IsMultiTenant)
            {
                return false;
            }

            if (tableResult.Description?.Equals(stellarDsTable.Description) is false)
            {
                return false;
            }

            foreach (var property in model.GetProperties())
            {
                var stellarDsType = property.GetCustomAttribute<StellarDsProperty>()?.Type;
                if (stellarDsType is null)
                {
                    return false;
                }

                if (!fieldResults.Any(f => f.Name.Equals(property.Name) && f.Type.Equals(stellarDsType)))
                {
                    return false;
                }
            }


            return true;
        }
    }
}
