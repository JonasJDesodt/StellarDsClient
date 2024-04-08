namespace StellarDsClient.Sdk.Abstractions
{
    public interface ITokenProvider
    {
        Task<string> Get();
    }
}