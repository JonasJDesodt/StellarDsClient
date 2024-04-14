namespace StellarDsClient.Builder.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StellarDsTable(bool isMultiTenant, string? description = null) : Attribute
    {
        public bool IsMultiTenant => isMultiTenant;

        public string? Description => description;
    }
}
