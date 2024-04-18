using System;

namespace StellarDsClient.Models.Request
{
    public class PutTaskRequest
    {
        public required string Title { get; set; }

        public required bool Done { get; set; }

        public required DateTime Updated { get; set; }
    }
}