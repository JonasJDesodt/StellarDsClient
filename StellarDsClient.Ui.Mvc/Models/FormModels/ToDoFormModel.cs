using System.ComponentModel.DataAnnotations;

namespace StellarDsClient.Ui.Mvc.Models.FormModels
{
    public class ToDoFormModel : BaseFormModel
    {
        [Required]
        [MaxLength(255)]
        public required string Title { get; set; }

        public bool Finished { get; set; }
    }
}