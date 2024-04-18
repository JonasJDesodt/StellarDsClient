using System.Net;
using StellarDsClient.Sdk.Dto.Transfer;

namespace StellarDsClient.Sdk.Extensions
{
    internal static class StellarDsResultExtensions
    {
        public static StellarDsResult Unauthorized(this StellarDsResult result)
        {
            result.AddUnauthorized();

            return result;
        }

        public static Dto.Transfer.StellarDsResult<T> Unauthorized<T>(this Dto.Transfer.StellarDsResult<T> result) where T : class
        {
            result.AddUnauthorized();

            return result;
        }

        private static void AddUnauthorized(this StellarDsResult result)
        {
            result.Messages =
            [
                new StellarDsErrorMessage()
                {
                    Code = HttpStatusCode.Unauthorized,
                    Message = "The refresh token has expired.",
                    Type = 0 // todo
                }
            ];
        }


        public static StellarDsResult TooManyRequests(this StellarDsResult result)
        {
            result.AddTooManyRequests();

            return result;
        }

        public static Dto.Transfer.StellarDsResult<T> TooManyRequests<T>(this Dto.Transfer.StellarDsResult<T> result) where T : class
        {
            result.AddTooManyRequests();

            return result;
        }

        private static void AddTooManyRequests(this StellarDsResult result)
        {
            result.Messages =
            [
                new StellarDsErrorMessage()
                {
                    Code = HttpStatusCode.TooManyRequests,
                    Message = "Rate limit reached.", //todo: add the stellardb error message?
                    Type = 0 // todo
                }
            ];
        }

        public static Dto.Transfer.StellarDsResult<T> ToNonNullable<T>(this Dto.Transfer.StellarDsResult<T>? result) where T : class
        {
            ArgumentNullException.ThrowIfNull(result);

            return result;
        }
    }
}