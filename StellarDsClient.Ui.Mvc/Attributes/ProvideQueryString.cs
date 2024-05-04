using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StellarDsClient.Ui.Mvc.Attributes
{
    public class ProvideQueryString(string key) : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is not Controller controller) return;

            var httpContext = context.HttpContext;

            controller.ViewData[key] = httpContext.Session.GetString(key);
        }
    }
}