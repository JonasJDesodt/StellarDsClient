using System.Net;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class HttpContextExtensions
    {
        public static HttpContext ClearOAuthCookies(this HttpContext context)
        {
            context.Response.Cookies.Delete("AccessToken");
            context.Response.Cookies.Delete("RefreshToken");

            return context;
        }
    }
}