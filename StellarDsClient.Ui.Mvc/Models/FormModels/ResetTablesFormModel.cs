using StellarDsClient.Ui.Mvc.Attributes;

namespace StellarDsClient.Ui.Mvc.Models.FormModels
{
    public class ResetTablesFormModel
    {
        public int? CurrentListTableId {get;set;}

        public int? CurrentToDoTableId { get; set; }

        [MatchesCurrentUsername]
        public required string Username { get; set; }
    }
}
