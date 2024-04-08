using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Extensions;
using StellarDsClient.Sdk.Settings;

//todo: api returns custom code: 'LimitReached' {"messages":[{"code":"LimitReached","message":"The requests limit of 500 has been reached. Upgrade your project tier to continue.","type":30}],"isSuccess":false}

namespace StellarDsClient.Sdk
{
    public class DataApiService<TTokenProvider>(IHttpClientFactory httpClientFactory, ApiSettings apiSettings, TTokenProvider tokenProvider) where TTokenProvider : ITokenProvider
    {
        private readonly string _requestUriBase = $"/{apiSettings.Version}/data/table";

        public async Task<StellarDsResult<IList<TResult>>> Find<TResult>(int tableId, string query) where TResult : class
        {
            return await GetAsync<IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableId) + query);
        }

        public async Task<StellarDsResult<TResult>> Get<TResult>(int tableId, int id) where TResult : class
        {   
            var result = await GetAsync<IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableId) + $"&whereQuery=id;equal;{id}");

            return new StellarDsResult<TResult>
            {
                Count = result.Count,
                IsSuccess = result.IsSuccess,
                Messages = result.Messages,
                Data = result.Data?.FirstOrDefault()
            };
        }

        public async Task<StellarDsResult<TResult>> Create<TRequest, TResult>(int tableId, TRequest request) where TRequest : class where TResult : class
        {
            var result = await PostAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableId), [request]);

            return new StellarDsResult<TResult>
            {
                Count = result.Count,
                IsSuccess = result.IsSuccess,
                Messages = result.Messages,
                Data = result.Data?.FirstOrDefault()
            };
        }

        public async Task<StellarDsResult<IList<TResult>>> Create<TRequest, TResult>(int tableId, IList<TRequest> requests) where TRequest : class where TResult : class
        {
            return await PostAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableId), requests);
        }

        public async Task<StellarDsResult<IList<TResult>>> Put<TRequest, TResult>(int tableId, int id, TRequest request) where TRequest : class where TResult : class
        {
            return await PutAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableId), [id], request);
        }

        public async Task<StellarDsResult<IList<TResult>>> Put<TRequest, TResult>(int tableId, IList<int> ids, TRequest request) where TRequest : class where TResult : class
        {
            return await PutAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableId), ids, request);
        }

        public async Task<StellarDsResult> Delete(int tableId, int id) // todo return StellarDsResult // todo use bulk endpoint
        {
            return await DeleteAsync(await GetHttpClientAsync(), GetDefaultRequestUri(tableId) + $"&record={id}");
        }

        public async Task Delete(int tableId, string[] ids)
        {
            if (ids.Length == 0)
            {
                return;
            }

            var httpClient = await GetHttpClientAsync();

            var httpResponse = await httpClient.PostAsJsonAsync(GetDeleteRequestUri(tableId), ids);  // expected json content to be e.g.  new  { records = int[]{1,2}}

            httpResponse.EnsureSuccessStatusCode();
        }

        public async Task<StreamProperties> UploadFileToApi(int tableId, string field, int record, MultipartFormDataContent content)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponse = await httpClient.PostAsync(GetBlobRequestUri(tableId, field, record), content);

            httpResponse.EnsureSuccessStatusCode();

            var result = await httpResponse.Content.ReadFromJsonAsync<StreamProperties>();

            return result ?? new StreamProperties(); //todo return nullable?
        }

        public async Task<byte[]> DownloadBlobFromApi(int tableId, string field, int record)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponse = await httpClient.GetAsync(GetBlobRequestUri(tableId, field, record));

            httpResponse.EnsureSuccessStatusCode();

            return await httpResponse.Content.ReadAsByteArrayAsync();
        }


        private string GetDefaultRequestUri(int tableId)
        {
            return $"{_requestUriBase}?project={apiSettings.Project}&table={tableId}";
        }

        private string GetDeleteRequestUri(int tableId)
        {
            return $"{_requestUriBase}/delete?project={apiSettings.Project}&table={tableId}";
        }

        private string GetBlobRequestUri(int tableId, string field, int record)
        {
            return $"{_requestUriBase}/blob?project={apiSettings.Project}&table={tableId}&field={field}&record={record}";
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            return httpClientFactory
                .CreateClient(apiSettings.Name)
                .AddAuthorization(await tokenProvider.Get());
        }

        private static async Task<StellarDsResult<TResult>> GetAsync<TResult>(HttpClient httpClient, string uri) where TResult : class
        {
            var httpResponseMessage = await httpClient.GetAsync(uri);

            var message = await httpResponseMessage.Content.ReadAsStringAsync();

            return await httpResponseMessage.ToStellarDsResult<TResult>();
        }

        private static async Task<StellarDsResult<TResult>> PostAsJsonAsync<TRequest, TResult>(HttpClient httpClient, string uri, IList<TRequest> requests) where TRequest : class where TResult : class
        {
            var httpResponseMessage = await httpClient.PostAsJsonAsync(uri, new { records = requests });

            return await httpResponseMessage.ToStellarDsResult<TResult>();
        }

        private static async Task<StellarDsResult<TResult>> PutAsJsonAsync<TRequest, TResult>(HttpClient httpClient, string uri, IList<int> ids, TRequest request) where TRequest : class where TResult : class
        {
            var httpResponseMessage = await httpClient.PutAsJsonAsync(uri, new { idList = ids, record = request });

            return await httpResponseMessage.ToStellarDsResult<TResult>();
        }

        private static async Task<StellarDsResult> DeleteAsync(HttpClient httpClient, string uri)
        {
            var httpResponse = await httpClient.DeleteAsync(uri);

            if (httpResponse.StatusCode is HttpStatusCode.Unauthorized)
            {
                return new StellarDsResult().Unauthorized();
            }

            httpResponse.EnsureSuccessStatusCode();

            return new StellarDsResult();
        }
    }
}