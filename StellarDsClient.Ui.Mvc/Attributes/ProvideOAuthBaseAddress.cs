using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Ui.Mvc.Attributes
{
    public class ProvideOAuthBaseAddress : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.RequestServices.GetService(typeof(OAuthSettings)) is not OAuthSettings oAuthSettings)
            {
                return;
            }

            if (context.Controller is not Controller controller)
            {
                return;
            }

            controller.ViewData["OAuthBaseAddress"] = oAuthSettings.BaseAddress;
        }
    }
}
