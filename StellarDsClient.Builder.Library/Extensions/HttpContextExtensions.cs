using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using StellarDsClient.Sdk;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class HttpContextExtensions
    {
        //todo: generic extension that throws exception if the service could not be retrieved?
        internal static OAuthApiService GetOauthApiService(this HttpContext httpContext)
        {
            if (httpContext.RequestServices.GetService<OAuthApiService>() is not { } oAuthApiService)
            {
                throw new NullReferenceException($"Unable to get the {nameof(OAuthApiService)}");
            }

            return oAuthApiService;
        }

        internal static string GetAuthorizationCode(this HttpContext httpContext)
        {
            var code = httpContext.Request.Query["code"].ToString();

            if (string.IsNullOrWhiteSpace(code.ToString()))
            {
                throw new NullReferenceException($"Unable to get the authorization code");
            }

            return code;
        }
    }
}
