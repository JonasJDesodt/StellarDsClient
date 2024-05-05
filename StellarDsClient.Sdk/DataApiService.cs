//todo: api returns custom code: 'LimitReached' {"messages":[{"code":"LimitReached","message":"The requests limit of 500 has been reached. Upgrade your project tier to continue.","type":30}],"isSuccess":false}

using System.Net;
using System.Net.Http.Json;
using System.Web;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Dto.Transfer;
using StellarDsClient.Sdk.Extensions;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Sdk
{
    public class DataApiService<TTokenProvider>(IHttpClientFactory httpClientFactory, TTokenProvider tokenProvider, StellarDsClientSettings stellarDsClientSettings) where TTokenProvider : ITokenProvider
    {
        private readonly string _requestUriBase = $"/{stellarDsClientSettings.ApiSettings.Version}/data/table";

        /// <summary>
        /// Gets records from a given table.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="table"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Dto.Transfer.StellarDsResult<IList<TResult>>> Find<TResult>(string table, string query) where TResult : class
        {
            return await GetAsync<IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(stellarDsClientSettings.ApiSettings.Tables[table].Id) + query);
        }

        /// <summary>
        /// Gets a single record with a given from a given table.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Dto.Transfer.StellarDsResult<TResult>> Get<TResult>(string table, int id) where TResult : class
        {
            var result = await GetAsync<IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(stellarDsClientSettings.ApiSettings.Tables[table].Id) + $"&whereQuery={HttpUtility.UrlEncode($"id;equal;{id}")}");

            return new Dto.Transfer.StellarDsResult<TResult>
            {
                Count = result.Count,
                IsSuccess = result.IsSuccess,
                Messages = result.Messages,
                Data = result.Data?.FirstOrDefault()
            };
        }

        /// <summary>
        /// Creates a single record in a given table.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="table"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Dto.Transfer.StellarDsResult<TResult>> Create<TRequest, TResult>(string table, TRequest request) where TRequest : class where TResult : class
        {
            var result = await PostAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(stellarDsClientSettings.ApiSettings.Tables[table].Id), [request]);

            return new Dto.Transfer.StellarDsResult<TResult>
            {
                Count = result.Count,
                IsSuccess = result.IsSuccess,
                Messages = result.Messages,
                Data = result.Data?.FirstOrDefault()
            };
        }

        /// <summary>
        /// Creates records in a given table.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="table"></param>
        /// <param name="requests"></param>
        /// <returns></returns>
        public async Task<Dto.Transfer.StellarDsResult<IList<TResult>>> Create<TRequest, TResult>(string table, IList<TRequest> requests) where TRequest : class where TResult : class
        {
            return await PostAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(stellarDsClientSettings.ApiSettings.Tables[table].Id), requests);
        }

        /// <summary>
        /// Updates a single record with a given id in a given table.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Dto.Transfer.StellarDsResult<IList<TResult>>> Put<TRequest, TResult>(string table, int id, TRequest request) where TRequest : class where TResult : class
        {
            return await PutAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(stellarDsClientSettings.ApiSettings.Tables[table].Id), [id], request);
        }

        /// <summary>
        /// Updates records with given ids in a given table.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tableId"></param>
        /// <param name="ids"></param>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<Dto.Transfer.StellarDsResult<IList<TResult>>> Put<TRequest, TResult>(int tableId, IList<int> ids, TRequest request) where TRequest : class where TResult : class
        {
            return await PutAsJsonAsync<TRequest, IList<TResult>>(await GetHttpClientAsync(), GetDefaultRequestUri(tableId), ids, request);
        }

        /// <summary>
        /// Deletes a single record with a given id in a given table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<StellarDsResult> Delete(string table, int id)
        {
            return await DeleteAsync(await GetHttpClientAsync(), GetDefaultRequestUri(stellarDsClientSettings.ApiSettings.Tables[table].Id) + $"&record={id}");
        }

        /// <summary>
        /// Deletes records with given ids in a given table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task Delete(string table, string[] ids)
        {
            if (ids.Length == 0)
            {
                return;
            }

            var httpClient = await GetHttpClientAsync();

            var httpResponse = await httpClient.PostAsJsonAsync(GetDeleteRequestUri(stellarDsClientSettings.ApiSettings.Tables[table].Id), ids);  // expected json content to be e.g.  new  { records = int[]{1,2}}

            httpResponse.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Uploads a blob to a field in a record of a given table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="record"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<StreamProperties> UploadFileToApi(string table, string field, int record, MultipartFormDataContent content)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponse = await httpClient.PostAsync(GetBlobRequestUri(stellarDsClientSettings.ApiSettings.Tables[table].Id, field, record), content);

            httpResponse.EnsureSuccessStatusCode();

            var result = await httpResponse.Content.ReadFromJsonAsync<StreamProperties>();

            return result ?? new StreamProperties(); //todo return nullable?
        }

        /// <summary>
        /// Downloads a blob from a given field in a given table for a single record.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadBlobFromApi(string table, string field, int record)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponse = await httpClient.GetAsync(GetBlobRequestUri(stellarDsClientSettings.ApiSettings.Tables[table].Id, field, record));

            httpResponse.EnsureSuccessStatusCode();

            return await httpResponse.Content.ReadAsByteArrayAsync();
        }

        private string GetDefaultRequestUri(int table)
        {
            return $"{_requestUriBase}?project={stellarDsClientSettings.ApiSettings.Project}&table={table}";
        }

        private string GetDeleteRequestUri(int table)
        {
            return $"{_requestUriBase}/delete?project={stellarDsClientSettings.ApiSettings.Project}&table={table}";
        }

        private string GetBlobRequestUri(int table, string field, int record)
        {
            return $"{_requestUriBase}/blob?project={stellarDsClientSettings.ApiSettings.Project}&table={table}&field={field}&record={record}";
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            return httpClientFactory
                .CreateClient(stellarDsClientSettings.ApiSettings.Name)
                .AddAuthorization(await tokenProvider.Get());
        }

        private static async Task<StellarDsResult<TResult>> GetAsync<TResult>(HttpClient httpClient, string uri) where TResult : class
        {
            var httpResponseMessage = await httpClient.GetAsync(uri);

            var flag = await httpResponseMessage.Content.ReadAsStringAsync();

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