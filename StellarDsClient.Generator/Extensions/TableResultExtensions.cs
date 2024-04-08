using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StellarDsClient.Dto.Schema;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Generator.Attributes;
using StellarDsClient.Generator.Models;

namespace StellarDsClient.Generator.Extensions
{
    internal static class TableResultExtensions
    {
        internal static TableResult? GetMetadata(this IList<TableResult> tables, string table)
        {
            return tables.FirstOrDefault(x => x.Name.Equals(table, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}