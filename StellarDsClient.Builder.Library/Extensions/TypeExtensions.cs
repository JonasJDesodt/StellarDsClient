using System.Reflection;
using StellarDsClient.Builder.Library.Attributes;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class TypeExtensions
    {
        internal static List<Type> EnsureStellarDsTableAnnotations(this List<Type> models)
        {

            //todo: create record with the settings + the model?
            models.ForEach(m =>
            {
                if (m.GetCustomAttribute<StellarDsTable>() is null)
                {
                    throw new NullReferenceException($"The {m.Name} model is not annotated with the {nameof(StellarDsTable)} attribute");
                }
            });

            return models;
        }
    }
}