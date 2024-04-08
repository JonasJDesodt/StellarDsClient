using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.EntityModels;
using StellarDsClient.Ui.Mvc.Models.Filters;
using StellarDsClient.Ui.Mvc.Models.PartialModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class TaskIndexViewModel 
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public PaginationPartialModel? PaginationPartialModel { get; set; }

        public ListEntityModel? ListEntity { get; set; }

        public IList<TaskResult>? TaskResults { get; set; }

        public TaskIndexFilter? TaskIndexFilter { get; set; }
    }
}