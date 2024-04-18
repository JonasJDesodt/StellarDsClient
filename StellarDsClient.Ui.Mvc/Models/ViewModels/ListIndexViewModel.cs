using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.PartialModels;
using StellarDsClient.Ui.Mvc.Models.UiModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class ListIndexViewModel 
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public PaginationPartialModel? PaginationPartialModel { get; set; }

        public IList<ListUiModel>? Lists { get; set; }

        public ListIndexFilter? ListIndexFilter { get; set; }
    }
}