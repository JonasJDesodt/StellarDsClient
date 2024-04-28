using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.FormModels;
using StellarDsClient.Ui.Mvc.Models.UiModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class ToDoCreateEditViewModel
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public ListUiModel? ListEntity { get; set; }

        public ToDoFormModel? TaskFormModel { get; set; }
    }
}