using StellarDsClient.Sdk.Dto.Transfer;

namespace StellarDsClient.Sdk.Extensions
{
    internal static class OAuthTokensExtensions
    {
        public static OAuthTokens ToNonNullable(this OAuthTokens? oAuthTokens)
        {
            ArgumentNullException.ThrowIfNull(oAuthTokens);

            return oAuthTokens;
        }
    }
}