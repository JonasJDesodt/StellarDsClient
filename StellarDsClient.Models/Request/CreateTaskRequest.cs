using System;

namespace StellarDsClient.Models.Request
{
    public class CreateTaskRequest
    {
        public required string Title { get; set; }

        public bool Done { get; set; }

        public required int ListId { get; set; }

        public required DateTime Created { get; set; }

        public required DateTime Updated { get; set; }
    }
}

