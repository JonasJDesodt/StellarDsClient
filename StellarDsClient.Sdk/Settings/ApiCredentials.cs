using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Sdk.Settings
{
    public class ApiCredentials
    {
        public required string Project { get; set; }

        public required string ReadOnlyToken { get; set; }
    }
}
