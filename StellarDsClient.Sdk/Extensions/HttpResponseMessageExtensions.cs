using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Sdk.Exceptions;

namespace StellarDsClient.Sdk.Extensions
{
    internal static class HttpResponseMessageExtensions
    {
        private static HttpResponseMessage RethrowResponseMessageException(this HttpResponseMessage httpResponseMessage)
        {
            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                switch (ex.StatusCode)
                {
                    //400
                    case HttpStatusCode.NotFound:
                        throw new CustomNotFoundException("Resource not found", ex);
                    case HttpStatusCode.Unauthorized:
                        throw new CustomUnauthorizedException("Unauthorized", ex);

                    //300
                    case HttpStatusCode.BadRequest:
                        throw new CustomBadRequestException("Bad request", ex);

                    case HttpStatusCode.OK:
                    case HttpStatusCode.Created:
                    case HttpStatusCode.Accepted:
                    case HttpStatusCode.NonAuthoritativeInformation:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.ResetContent:
                    case HttpStatusCode.PartialContent:
                    case HttpStatusCode.MultiStatus:
                    case HttpStatusCode.AlreadyReported:
                    case HttpStatusCode.IMUsed:
                        break;

                    //100
                    case HttpStatusCode.Continue:
                    case HttpStatusCode.SwitchingProtocols:
                    case HttpStatusCode.Processing:
                    case HttpStatusCode.EarlyHints:

                    //300
                    case HttpStatusCode.Ambiguous:
                    case HttpStatusCode.Moved:
                    case HttpStatusCode.Found:
                    case HttpStatusCode.RedirectMethod:
                    case HttpStatusCode.NotModified:
                    case HttpStatusCode.UseProxy:
                    case HttpStatusCode.Unused:
                    case HttpStatusCode.RedirectKeepVerb:
                    case HttpStatusCode.PermanentRedirect:

                    //400
                    case HttpStatusCode.PaymentRequired:
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.MethodNotAllowed:
                    case HttpStatusCode.NotAcceptable:
                    case HttpStatusCode.ProxyAuthenticationRequired:
                    case HttpStatusCode.RequestTimeout:
                    case HttpStatusCode.Conflict:
                    case HttpStatusCode.Gone:
                    case HttpStatusCode.LengthRequired:
                    case HttpStatusCode.PreconditionFailed:
                    case HttpStatusCode.RequestEntityTooLarge:
                    case HttpStatusCode.RequestUriTooLong:
                    case HttpStatusCode.UnsupportedMediaType:
                    case HttpStatusCode.RequestedRangeNotSatisfiable:
                    case HttpStatusCode.ExpectationFailed:
                    case HttpStatusCode.MisdirectedRequest:
                    case HttpStatusCode.UnprocessableEntity:
                    case HttpStatusCode.Locked:
                    case HttpStatusCode.FailedDependency:
                    case HttpStatusCode.UpgradeRequired:
                    case HttpStatusCode.PreconditionRequired:
                    case HttpStatusCode.TooManyRequests:
                    case HttpStatusCode.RequestHeaderFieldsTooLarge:
                    case HttpStatusCode.UnavailableForLegalReasons:

                    //500
                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.NotImplemented:
                    case HttpStatusCode.BadGateway:
                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.GatewayTimeout:
                    case HttpStatusCode.HttpVersionNotSupported:
                    case HttpStatusCode.VariantAlsoNegotiates:
                    case HttpStatusCode.InsufficientStorage:
                    case HttpStatusCode.LoopDetected:
                    case HttpStatusCode.NotExtended:
                    case HttpStatusCode.NetworkAuthenticationRequired:
                    case null:
                    default:
                        throw new CustomHttpException("An error occurred while fetching data", ex);
                }
            }

            return httpResponseMessage;
        }

        public static async Task<StellarDsResult<TResult>> ToStellarDsResult<TResult>(this HttpResponseMessage httpResponseMessage) where TResult : class
        {
            var result = await httpResponseMessage
                .RethrowResponseMessageException()
                .Content
                .ReadFromJsonAsync<StellarDsResult<TResult>>();

            return result.ToNonNullable();
        }

        public static async Task<OAuthTokens> ToOAuthTokens(this HttpResponseMessage httpResponseMessage)
        {
            var result = await httpResponseMessage
                 .EnsureSuccessStatusCode()
                 .Content
                 .ReadFromJsonAsync<OAuthTokens>();

            return result.ToNonNullable();
        }
    }
}