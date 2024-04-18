using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using StellarDsClient.Sdk.Dto.Transfer;

namespace StellarDsClient.Sdk.Extensions
{
    internal static class HttpResponseMessageExtensions
    {
        //todo: api returns custom code: 'LimitReached' {"messages":[{"code":"LimitReached","message":"The requests limit of 500 has been reached. Upgrade your project tier to continue.","type":30}],"isSuccess":false}

        public static HttpResponseMessage EnsureSerializableContent(this HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.StatusCode is not HttpStatusCode.TooManyRequests or HttpStatusCode.Unauthorized)
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }

            return httpResponseMessage;
        }

        public static async Task<Dto.Transfer.StellarDsResult<TResult>> ToStellarDsResult<TResult>(this HttpResponseMessage httpResponseMessage) where TResult : class
        {
            var result = await httpResponseMessage
                .EnsureSerializableContent()
                .Content
                .ReadFromJsonAsync<Dto.Transfer.StellarDsResult<TResult>>();

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