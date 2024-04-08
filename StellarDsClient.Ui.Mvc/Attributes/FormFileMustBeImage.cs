using System.ComponentModel.DataAnnotations;

namespace StellarDsClient.Ui.Mvc.Attributes
{
    public class FormFileMustBeImage : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                // List of common image content types
                var imageContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

                // Check if the file's content type matches any of the image content types
                if (!imageContentTypes.Contains(file.ContentType.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            else
            {
                // You can return a success result or an error if you require a file to be uploaded.
                // To force a file to be uploaded, return a new ValidationResult with an appropriate message instead.
                return ValidationResult.Success;
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return "The file must be an image (JPEG, PNG, GIF, BMP, WEBP).";
        }
    }
}
