using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.FormModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class ListCreateEditViewModel
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public ListFormModel? ListFormModel { get; set; }
    }
}