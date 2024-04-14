namespace StellarDsClient.Builder.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class StellarDsProperty(string type) : Attribute
    {
        internal string Type => type;
    }
}
