namespace StellarDsClient.Dto.Data.Request
{
    public class CreateTaskRequest
    {
        public required string Title { get; set; }

        public bool Done { get; set; }

        public required int ListId { get; set; }

        public required DateTime Created { get; set; }
    }
}