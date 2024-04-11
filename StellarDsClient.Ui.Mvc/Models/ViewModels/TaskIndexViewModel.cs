using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.PartialModels;
using StellarDsClient.Ui.Mvc.Models.UiModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class TaskIndexViewModel 
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public PaginationPartialModel? PaginationPartialModel { get; set; }

        public ListUiModel? List { get; set; }

        public TaskIndexFilter? TaskIndexFilter { get; set; }
    }
}