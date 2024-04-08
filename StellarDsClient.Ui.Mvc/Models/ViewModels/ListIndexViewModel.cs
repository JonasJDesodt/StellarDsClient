using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.EntityModels;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.PartialModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class ListIndexViewModel 
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public PaginationPartialModel? PaginationPartialModel { get; set; }

        public IList<ListEntityModel>? ListEntities { get; set; }

        public ListIndexFilter? ListIndexFilter { get; set; }
    }
}