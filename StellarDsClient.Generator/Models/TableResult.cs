using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Generator.Models
{
    internal class TableResult
    {
        public required int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public bool IsMultitenant { get; set; }
    }
}