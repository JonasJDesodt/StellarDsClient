namespace StellarDsClient.Builder.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class StellarDsType(string name) : Attribute
    {
        internal string Name => name;
    }
}
