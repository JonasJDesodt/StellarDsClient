using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Sdk.Settings
{
    public interface IStellarDsClientSettings
    {
        IApiSettings ApiSettings { get; set; }

        IOAuthSettings OAuthSettings { get; set; }
    }
}
