using StellarDsClient.Sdk.Abstractions;

namespace StellarDsClient.Builder.Library.Providers
{
    internal class AccessTokenProvider : ITokenProvider
    {
        private string? _accessToken;

        public Task<string> Get()
        {
            return Task.FromResult(_accessToken ?? throw new NullReferenceException()); //todo: handle this better
        }

        public void Set(string accessToken)
        {
            _accessToken = accessToken;
        }
    }
}