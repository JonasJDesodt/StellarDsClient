using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Generator.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class StellarDsType(string name) : Attribute
    {
        internal string Name => name;
    }
}
