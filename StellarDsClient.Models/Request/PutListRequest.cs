using System;

namespace StellarDsClient.Models.Request
{
    public class PutListRequest
    {
        public required string Title { get; set; }

        public DateTime? Deadline { get; set; }

        public required DateTime Updated { get; set; }
    }
}