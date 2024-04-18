using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.UiModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class HomeViewModel
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public ListUiModel? List { get; set; }
    }
}