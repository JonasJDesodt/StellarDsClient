namespace StellarDsClient.Sdk.Dto.Transfer
{
    public class StellarDsResult
    {
        public IList<Sdk.Dto.Transfer.StellarDsErrorMessage> Messages { get; set; } = [];

        public bool? IsSuccess { get; set; }
    }

    public class StellarDsResult<T> : StellarDsResult where T : class
    {
        public int Count { get; set; }

        public T? Data { get; set; }
    }
}