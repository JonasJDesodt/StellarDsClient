namespace StellarDsClient.Dto.Data.Request
{
    public class PutListRequest
    {
        public required string Title { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
