using System.Net;

namespace StellarDsClient.Dto.Transfer
{
    public class StellarDsErrorMessage
    {
        public required HttpStatusCode Code { get; set; }

        public required string Message { get; set; }

        public required int Type { get; set; }
    }
}
