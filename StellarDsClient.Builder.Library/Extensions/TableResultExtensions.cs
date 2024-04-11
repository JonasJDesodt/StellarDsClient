using StellarDsClient.Dto.Schema;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class TableResultExtensions
    {
        internal static TableResult? GetMetadata(this IList<TableResult> tables, string table)
        {
            return tables.FirstOrDefault(x => x.Name.Equals(table, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}