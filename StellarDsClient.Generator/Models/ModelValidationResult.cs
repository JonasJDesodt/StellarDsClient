using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Generator.Models
{
    internal class ModelValidationResult
    {
        public bool IsValid { get; set; } = false;

        public bool IsError { get; set; } = false;
    }
}
