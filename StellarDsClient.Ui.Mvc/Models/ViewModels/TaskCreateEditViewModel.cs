using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.EntityModels;
using StellarDsClient.Ui.Mvc.Models.FormModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class TaskCreateEditViewModel
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public ListEntityModel? ListEntity { get; set; }

        public TaskFormModel? TaskFormModel { get; set; }
    }
}