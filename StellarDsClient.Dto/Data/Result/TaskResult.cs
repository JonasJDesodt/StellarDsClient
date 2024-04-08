﻿namespace StellarDsClient.Dto.Data.Result
{
    public class TaskResult
    {
        public required int Id { get; set; }

        public required string Title { get; set; }

        public required bool Done { get; set; }

        public required int ListId { get; set; }

        public required DateTime Created { get; set; }
    }
}