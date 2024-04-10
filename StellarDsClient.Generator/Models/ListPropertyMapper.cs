using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarDsClient.Generator.Attributes;

namespace StellarDsClient.Generator.Models
{
    internal class ListPropertyMapper
    {
        [StellarDsType("NVarChar(255)")]
        public required string Title { get; set; }

        [StellarDsType("DateTime")]
        public required DateTime Created { get; set; }

        [StellarDsType("DateTime")]
        public required DateTime Updated { get; set; }

        [StellarDsType("DateTime")]
        public DateTime? Deadline { get; set; }

        [StellarDsType("Blob")]
        public string? Image { get; set; }

        [StellarDsType("NVarChar(255)")]
        public required string OwnerId { get; set; }

        [StellarDsType("NVarChar(255)")]
        public required string OwnerName { get; set; }
    }
}