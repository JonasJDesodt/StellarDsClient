using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Sdk.Settings
{
    public interface IApiSettings
    {
        string Name { get; set; }

        string BaseAddress { get; set; }

        string Version { get; set; }

        string Project { get; set; }

        string ReadOnlyToken { get; set; }

        IDictionary<string, ITableSettings> Tables { get; set; }
    }
}