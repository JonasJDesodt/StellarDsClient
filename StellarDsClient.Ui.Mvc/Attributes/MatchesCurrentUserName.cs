using StellarDsClient.Ui.Mvc.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace StellarDsClient.Ui.Mvc.Attributes
{
    public class MatchesCurrentUserName : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.GetService<IHttpContextAccessor>()?.HttpContext?.User is not { } user)
            {
                return new ValidationResult("Unable to verify the username.");
            }

            if (value is not string userName )
            {
                return new ValidationResult("The username does not match.");
            }

            if (userName.Equals(user.GetName()))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The username does not match.");
        }
    }
}
