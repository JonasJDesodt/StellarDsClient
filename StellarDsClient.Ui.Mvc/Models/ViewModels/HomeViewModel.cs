using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.UiModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class HomeViewModel
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public ListUiModel? List { get; set; }
    }
}