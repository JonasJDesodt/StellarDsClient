using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.UiModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class TaskCreateEditViewModel
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public ListUiModel? ListEntity { get; set; }

        public TaskFormModel? TaskFormModel { get; set; }
    }
}