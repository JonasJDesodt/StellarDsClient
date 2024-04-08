using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Dto.Data.Result
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

        public required int TaskId { get; set; }

        public required string TaskTitle { get; set; }

        public required bool TaskFinished { get; set; }

        public required int TaskListId { get; set; }

        public required DateTime TaskCreated { get; set; }
    }
}