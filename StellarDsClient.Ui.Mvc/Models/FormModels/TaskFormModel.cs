using System.ComponentModel.DataAnnotations;

namespace StellarDsClient.Ui.Mvc.Models.FormModels
{
    public class TaskFormModel : BaseFormModel
    {
        [Required]
        [MaxLength(255)]
        public required string Title { get; set; }

        public bool Finished { get; set; }
    }
}