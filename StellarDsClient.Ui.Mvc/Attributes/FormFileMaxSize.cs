using System.ComponentModel.DataAnnotations;
using StellarDsClient.Ui.Mvc.Models.FormModels;

namespace StellarDsClient.Ui.Mvc.Attributes
{
    public class FormFileMaxSize(int maxFileSizeInKilobytes) : ValidationAttribute
    {
        private readonly int _maxFileSizeInBytes = maxFileSizeInKilobytes * 1024; // Convert KB to Bytes
        private const int MinFileSizeInBytes = 1; // Minimum file size in bytes

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is not IFormFile formFile) return ValidationResult.Success; 

            if (formFile.Length > _maxFileSizeInBytes)
            {
                return new ValidationResult(GetErrorMessageForMaxSize());
            }

            return formFile.Length < MinFileSizeInBytes ? new ValidationResult(GetErrorMessageForMinSize()) : ValidationResult.Success;
        }

        public string GetErrorMessageForMaxSize()
        {
            return $"The maximum allowed file size is {_maxFileSizeInBytes / 1024} kB.";
        }

        public string GetErrorMessageForMinSize()
        {
            return "The file size must be greater than 0 bytes.";
        }
    }
}