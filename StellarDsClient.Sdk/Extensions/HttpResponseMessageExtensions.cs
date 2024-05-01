using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static HttpResponseMessage LogResponseMessageException(this HttpResponseMessage httpResponseMessage)
        {
            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex); //todo: log it!
            }

            return httpResponseMessage;
        }

        public static async Task<StellarDsResult<TResult>> ToStellarDsResult<TResult>(this HttpResponseMessage httpResponseMessage) where TResult : class
        {
            var result = await httpResponseMessage
                .LogResponseMessageException()
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