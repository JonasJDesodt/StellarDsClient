//todo: api returns custom code: 'LimitReached' {"messages":[{"code":"LimitReached","message":"The requests limit of 500 has been reached. Upgrade your project tier to continue.","type":30}],"isSuccess":false}

using System.Net;
using System.Net.Http.Json;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Extensions;
using StellarDsClient.Sdk.Models;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Sdk
{
    public class DataApiService<TTokenProvider>(IHttpClientFactory httpClientFactory, ApiSettings apiSettings, TTokenProvider tokenProvider, TableSettingsDictionary tableSettings) where TTokenProvider : ITokenProvider
    {
        private readonly string _requestUriBase = $"/{apiSettings.Version}/data/table";

        public async Task<StellarDsResult<IList<TResult>>> Find<TResult>(string table, string query) where TResult : class
        {
            return await GetAsync<IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableSettings[table]) + query);
        }

        public async Task<StellarDsResult<TResult>> Get<TResult>(string table, int id) where TResult : class
        {   
            var result = await GetAsync<IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableSettings[table]) + $"&whereQuery=id;equal;{id}");

            return new StellarDsResult<TResult>
            {
                Count = result.Count,
                IsSuccess = result.IsSuccess,
                Messages = result.Messages,
                Data = result.Data?.FirstOrDefault()
            };
        }

        public async Task<StellarDsResult<TResult>> Create<TRequest, TResult>(string table, TRequest request) where TRequest : class where TResult : class
        {
            var result = await PostAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableSettings[table]), [request]);

            return new StellarDsResult<TResult>
            {
                Count = result.Count,
                IsSuccess = result.IsSuccess,
                Messages = result.Messages,
                Data = result.Data?.FirstOrDefault()
            };
        }

        public async Task<StellarDsResult<IList<TResult>>> Create<TRequest, TResult>(string table, IList<TRequest> requests) where TRequest : class where TResult : class
        {
            return await PostAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableSettings[table]), requests);
        }

        public async Task<StellarDsResult<IList<TResult>>> Put<TRequest, TResult>(string table, int id, TRequest request) where TRequest : class where TResult : class
        {
            return await PutAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableSettings[table]), [id], request);
        }

        public async Task<StellarDsResult<IList<TResult>>> Put<TRequest, TResult>(int tableId, IList<int> ids, TRequest request) where TRequest : class where TResult : class
        {
            return await PutAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableId), ids, request);
        }

        public async Task<StellarDsResult> Delete(string table, int id) 
        {
            return await DeleteAsync(await GetHttpClientAsync(), GetDefaultRequestUri(tableSettings[table]) + $"&record={id}");
        }

        public async Task Delete(string table, string[] ids)
        {
            if (ids.Length == 0)
            {
                return;
            }

            var httpClient = await GetHttpClientAsync();

            var httpResponse = await httpClient.PostAsJsonAsync(GetDeleteRequestUri(tableSettings[table]), ids);  // expected json content to be e.g.  new  { records = int[]{1,2}}

            httpResponse.EnsureSuccessStatusCode();
        }

        public async Task<StreamProperties> UploadFileToApi(string table, string field, int record, MultipartFormDataContent content)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponse = await httpClient.PostAsync(GetBlobRequestUri(tableSettings[table], field, record), content);

            httpResponse.EnsureSuccessStatusCode();

            var result = await httpResponse.Content.ReadFromJsonAsync<StreamProperties>();

            return result ?? new StreamProperties(); //todo return nullable?
        }

        public async Task<byte[]> DownloadBlobFromApi(string table, string field, int record)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponse = await httpClient.GetAsync(GetBlobRequestUri(tableSettings[table], field, record));

            httpResponse.EnsureSuccessStatusCode();

            return await httpResponse.Content.ReadAsByteArrayAsync();
        }


        private string GetDefaultRequestUri(int table)
        {
            return $"{_requestUriBase}?project={apiSettings.Project}&table={table}";
        }

        private string GetDeleteRequestUri(int table)
        {
            return $"{_requestUriBase}/delete?project={apiSettings.Project}&table={table}";
        }

        private string GetBlobRequestUri(int table, string field, int record)
        {
            return $"{_requestUriBase}/blob?project={apiSettings.Project}&table={table}&field={field}&record={record}";
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