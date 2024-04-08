namespace StellarDsClient.Dto.Schema
{
    public class TableResult
    {
        public required int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public bool IsMultitenant { get; set; }
    }
}