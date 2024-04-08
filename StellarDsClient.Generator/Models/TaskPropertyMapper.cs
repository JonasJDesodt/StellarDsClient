using StellarDsClient.Generator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Generator.Models
{
    internal class TaskPropertyMapper
    {
        [StellarDsType("NVarChar(255)")]
        public required string Title { get; set; }

        [StellarDsType("Boolean")]
        public bool Done { get; set; }

        [StellarDsType("Int")]
        public required int ListId { get; set; }

        [StellarDsType("DateTime")]
        public required DateTime Created { get; set; }
    }
}