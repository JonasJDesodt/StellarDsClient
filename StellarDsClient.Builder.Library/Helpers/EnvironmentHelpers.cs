namespace StellarDsClient.Builder.Library.Helpers
{
    internal static class EnvironmentHelpers
    {
        internal static string GetApplicationUrl()
        {
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";");

            var applicationUrl = urls?.Single(x => x.StartsWith("https://"));

            if (string.IsNullOrWhiteSpace(applicationUrl))
            {
                throw new NullReferenceException("Unable to retrieve the application url from launchsettings.json");
            }

            //todo: test the localhostport?

            return applicationUrl;
        }
    }
}