using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StellarDsClient.Ui.Mvc.Attributes
{
    public class StoreQueryString : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is not { } controller) return;
                
            var httpContext = context.HttpContext;

            httpContext.Session.SetString(controller.GetType().Name, httpContext.Request.QueryString.ToString());
        }
    }
}
