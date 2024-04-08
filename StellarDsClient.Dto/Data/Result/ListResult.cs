namespace StellarDsClient.Dto.Data.Result
{
    public class ListResult
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required DateTime Created { get; set; }

        public DateTime? Deadline { get; set; }

        public string? Image { get; set; }

        public required string OwnerId { get; set; }

        public required string OwnerName { get; set; }
    }
}