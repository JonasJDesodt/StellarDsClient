namespace StellarDsClient.Dto.Data.Request
{
    public class CreateListRequest
    {
        public required string Title { get; set; }

        public required DateTime Created { get; set; }

        public required DateTime Updated { get; set; }

        public DateTime? Deadline { get; set; }

        public required string OwnerId { get; set; }

        public required string OwnerName { get; set; }
    }
}