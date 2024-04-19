namespace StellarDsClient.Sdk.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StellarDsTable(bool isMultiTenant, string? description = null) : Attribute //todo: internal?
    {
        public bool IsMultiTenant => isMultiTenant;

        public string? Description => description;
    }
}
