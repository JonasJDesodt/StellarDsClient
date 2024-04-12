using StellarDsClient.Ui.Mvc.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StellarDsClient.Ui.Mvc.Models.FormModels
{
    public class ListFormModel : BaseFormModel
    {
        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }

        [DisallowHistoricDatetime]
        public DateTime? Deadline { get; set; }

        [FormFileMaxSize(25)]
        [FormFileMustBeImage]
        public IFormFile? ImageUpload { get; set; }

        public byte[]? CurrentImage { get; set; }
    }
}