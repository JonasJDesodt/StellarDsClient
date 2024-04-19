﻿namespace StellarDsClient.Sdk.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StellarDsProperty(string type) : Attribute //todo: internal? or move to sdk?
    {
       public string Type => type;
    }
}
