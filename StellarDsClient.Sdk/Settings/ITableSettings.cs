using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Sdk.Settings
{
    public interface ITableSettings
    {
        string Name { get; set; }

        int Id { get; set; }
    }
}
