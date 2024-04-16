using StellarDsClient.Dto.Schema;
using StellarDsClient.Dto.Transfer;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Extensions;
using StellarDsClient.Sdk.Settings;
using System.Net.Http.Json;

namespace StellarDsClient.Sdk
{
    public class SchemaApiService<TTokenProvider>(IHttpClientFactory httpClientFactory, ApiSettings apiSettings, TTokenProvider tokenProvider) where TTokenProvider : ITokenProvider
    {
        public async Task<StellarDsResult<IList<TableResult>>> FindTables()
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.GetAsync($"v1/schema/table?project={apiSettings.Project}");
  
            return await httpResponseMessage.ToStellarDsResult<IList<TableResult>>();
        }

        public async Task<StellarDsResult<TableResult>> GetTable(int id)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.GetAsync($"v1/schema/table?project={apiSettings.Project}&table={id}");

            return await httpResponseMessage.ToStellarDsResult<TableResult>();
        }

        //todo: create request
        public async Task<StellarDsResult<TableResult>> CreateTable(string title, string? description, bool isMultiTenant)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.PostAsJsonAsync($"v1/schema/table?project={apiSettings.Project}", new { name = title, description, isMultitenant = isMultiTenant });

            return await httpResponseMessage.ToStellarDsResult<TableResult>();
        }
        
        public async Task<StellarDsResult<IList<FieldResult>>> GetFields(int tableId)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.GetAsync($"v1/schema/table/field?project={apiSettings.Project}&table={tableId}");

            return await httpResponseMessage.ToStellarDsResult<IList<FieldResult>>();
        }


        public async Task<StellarDsResult<FieldResult>> CreateField(int tableId, string title, string stellarDsType)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.PostAsJsonAsync($"v1/schema/table/field?project={apiSettings.Project}&table={tableId}", new { name = title, type = stellarDsType });

            return await httpResponseMessage.ToStellarDsResult<FieldResult>();
        }

        public async Task DeleteTable(int id)
        {
            var httpClient = await GetHttpClientAsync();

            (await httpClient.DeleteAsync($"v1/schema/table?project={apiSettings.Project}&table={id}")).EnsureSuccessStatusCode();
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            return httpClientFactory
                .CreateClient(apiSettings.Name)
                .AddAuthorization(await tokenProvider.Get());
        }
    }
}
