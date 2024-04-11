using StellarDsClient.Dto.Data.Result;

namespace StellarDsClient.Ui.Mvc.Models.UiModels
{
    public class ListUiModel
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required DateTime Created { get; set; }

        public DateTime? Deadline { get; set; }

        public byte[]? Image { get; set; }

        public required string OwnerId { get; set; }

        public required string OwnerName { get; set; }

        public IList<TaskResult> TaskResults = [];
    }
}