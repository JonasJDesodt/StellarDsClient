using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Sdk.Settings
{
    public interface IOAuthSettings
    {
        string Name { get; set; }

        string BaseAddress { get; set; }

        string ClientId { get; set; }

        string ClientSecret { get; set; }

        string RedirectUri { get; set; }
    }
}
