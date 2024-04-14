using StellarDsClient.Builder.Library.Attributes;
using StellarDsClient.Dto.Schema;
using System.Reflection;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class FieldResultExtensions
    {
        internal static bool Validate(this IList<FieldResult> fieldResults, Type model)
        {
            foreach (var property in model.GetProperties())
            {
                var stellarDsType = property.GetCustomAttribute<StellarDsProperty>()?.Type ?? property.PropertyType.ToString();

                if (fieldResults.Count(x => x.Name.Equals(property.Name.ToLowerInvariant()) && x.Type.Equals(stellarDsType)) != 1)
                {
                    return false;
                }
            }

            return true;
        }
    }
}