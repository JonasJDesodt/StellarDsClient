using System;

namespace StellarDsClient.Models.Result
{
    public class ListTaskResult
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required DateTime Created { get; set; }

        public DateTime? Deadline { get; set; }

        public string? Image { get; set; }

        public required string OwnerId { get; set; }

        public required string OwnerName { get; set; }

        //public required int TaskId { get; set; }

        public required string TaskTitle { get; set; }

        public required bool TaskDone { get; set; }

        public required int TaskListId { get; set; }

        public required DateTime TaskCreated { get; set; }

        public required DateTime TaskUpdated { get; set; }
    }
}