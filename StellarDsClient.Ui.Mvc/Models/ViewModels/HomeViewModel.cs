using StellarDsClient.Dto.Data.Result;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Ui.Mvc.Models.EntityModels;

namespace StellarDsClient.Ui.Mvc.Models.ViewModels
{
    public class HomeViewModel
    {
        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }

        public ListEntityModel? ListEntity { get; set; }
    }
}