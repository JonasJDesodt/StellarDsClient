using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Helpers
{
    internal class AppSettingsHelpers
    {
        internal static ApiSettings RequestApiSettings()
        {
            Console.Write("Project id: ");
            var projectId = Console.ReadLine();
            if (!Guid.TryParse(projectId, out _))
            {
                Console.WriteLine("The provided project id is not a valid GUID");

                Environment.Exit(0);
            }

            Console.Write("Readonly access token: ");
            var readOnlyAccessToken = Console.ReadLine();
            try
            {
                ArgumentNullException.ThrowIfNull(readOnlyAccessToken);

                new JsonWebTokenHandler().ReadJsonWebToken(readOnlyAccessToken);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                Environment.Exit(0);
            }

            return new ApiSettings
            {
                BaseAddress = "https://api.stellards.io",
                Name = "StellarDs",
                Project = projectId,
                ReadOnlyToken = readOnlyAccessToken,
                Version = "v1"
            };
        }

        internal static OAuthSettings RequestOAuthSettings(string applicationUrl)
        {
            Console.Write("Client Id: ");
            var clientId = Console.ReadLine();
            if (!Guid.TryParse(clientId, out _))
            {
                Console.WriteLine("The provided client id is not a valid GUID");

                Environment.Exit(0);
            }

            Console.Write("Client secret: ");
            var clientSecret = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                Console.WriteLine("Please provide a client secret");

                Environment.Exit(0);
            }

            return new OAuthSettings
            {
                BaseAddress = "https://stellards.io",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Name = "OAuth",
                RedirectUri = $"{applicationUrl}/oauth/oauthcallback",
            };
        }

        internal static int RequestLocalhostPort()
        {
            Console.Write("Localhost port: ");
            var localHostPort = Console.ReadLine();

            if (int.TryParse(localHostPort, out var port))
            {
                return port;
            }

            Console.WriteLine("The provided localhost port is not an integer");

            Environment.Exit(0);

            return port;
        }
    }
}