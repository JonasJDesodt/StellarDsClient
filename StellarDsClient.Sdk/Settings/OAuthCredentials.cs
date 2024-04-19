using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Sdk.Settings
{
    public class OAuthCredentials
    {
        public required string ClientId { get; set; }

        public required string ClientSecret { get; set; }

        public required string RedirectUri { get; set; }
    }
}