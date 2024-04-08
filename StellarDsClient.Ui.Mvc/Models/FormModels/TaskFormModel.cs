using System.ComponentModel.DataAnnotations;

namespace StellarDsClient.Ui.Mvc.Models.FormModels
{
    public class TaskFormModel : BaseFormModel
    {
        [Required]
        public required string Title { get; set; }

        public bool Finished { get; set; }
    }
}