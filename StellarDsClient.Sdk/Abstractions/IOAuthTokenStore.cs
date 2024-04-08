namespace StellarDsClient.Sdk.Abstractions
{
    public interface IOAuthTokenStore
    {
        string? GetAccessToken();

        string GetRefreshToken();

        void SaveAccessToken(string token, DateTimeOffset expires);

        void SaveRefreshToken(string token, DateTimeOffset expires);
    }
}