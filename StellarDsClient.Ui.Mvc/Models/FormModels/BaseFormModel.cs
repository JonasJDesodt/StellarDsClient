using StellarDsClient.Dto.Transfer;

namespace StellarDsClient.Ui.Mvc.Models.FormModels
{
    public class BaseFormModel 
    {
        public int Id { get; set; }

        public bool HasDeleteRequest { get; set; }

        public IList<StellarDsErrorMessage>? ErrorMessages { get; set; }
    }
}
