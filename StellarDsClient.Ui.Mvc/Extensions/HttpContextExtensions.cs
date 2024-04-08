using StellarDsClient.Ui.Mvc.Models.Settings;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class HttpContextExtensions
    {
        public static HttpContext ClearApplicationCookies(this HttpContext context)
        {
            if (context.RequestServices.GetService<IConfiguration>() is not { } configuration)
            {
                return context;
            }

            var cookieSettingsSection = configuration.GetSection(nameof(CookieSettings));

            foreach (var type in cookieSettingsSection.GetChildren())
            {
                foreach (var cookie in type.GetChildren())
                {
                    context.Response.Cookies.Delete(cookie.Key);
                }
            }

            return context;
        }

        //todo: test
        public static IList<string> GetCookieKeys(this HttpContext context)
        {
            if (context.RequestServices.GetService<IConfiguration>() is not { } configuration)
            {
                return [];
            }

            var keys = new List<string>();

            var cookieSettingsSection = configuration.GetSection(nameof(CookieSettings));

            foreach (var type in cookieSettingsSection.GetChildren())
            {
                foreach (var cookie in type.GetChildren())
                {
                    keys.Add(cookie.Key);
                }
            }

            return keys;
        }

        public static IList<string> GetCookieKeys(this HttpContext context, string section)
        {
            if (context.RequestServices.GetService<IConfiguration>() is not { } configuration)
            {
                return [];
            }

            return configuration
                .GetSection(nameof(CookieSettings))
                .GetSection(section)
                .GetChildren()
                .Select(cookie => cookie.Key)
                .ToList();
        }
    }
}
