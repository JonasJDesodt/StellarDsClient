using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Extensions;
using StellarDsClient.Sdk.Settings;
using System.Net.Http.Json;
using System.Reflection;
using StellarDsClient.Sdk.Attributes;
using StellarDsClient.Sdk.Dto.Schema;
using StellarDsClient.Sdk.Dto.Transfer;


namespace StellarDsClient.Sdk
{
    public class SchemaApiService<TTokenProvider>(IHttpClientFactory httpClientFactory, ApiSettings apiSettings, ApiCredentials apiCredentials, TTokenProvider tokenProvider) where TTokenProvider : ITokenProvider
    {
        public async Task<StellarDsResult<IList<TableResult>>> FindTables()
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.GetAsync($"v1/schema/table?project={apiCredentials.Project}");

            return await httpResponseMessage.ToStellarDsResult<IList<TableResult>>();
        }

        public async Task<StellarDsResult<TableResult>> GetTable(int id)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.GetAsync($"v1/schema/table?project={apiCredentials.Project}&table={id}");

            return await httpResponseMessage.ToStellarDsResult<TableResult>();
        }

        //todo: create request
        public async Task<StellarDsResult<TableResult>> CreateTable(string title, string? description, bool isMultiTenant)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.PostAsJsonAsync($"v1/schema/table?project={apiCredentials.Project}", new { name = title, description, isMultitenant = isMultiTenant });

            return await httpResponseMessage.ToStellarDsResult<TableResult>();
        }

        public async Task<StellarDsResult<IList<FieldResult>>> GetFields(int tableId)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.GetAsync($"v1/schema/table/field?project={apiCredentials.Project}&table={tableId}");

            return await httpResponseMessage.ToStellarDsResult<IList<FieldResult>>();
        }


        public async Task<StellarDsResult<FieldResult>> CreateField(int tableId, string title, string stellarDsType)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.PostAsJsonAsync($"v1/schema/table/field?project={apiCredentials.Project}&table={tableId}", new { name = title, type = stellarDsType });

            return await httpResponseMessage.ToStellarDsResult<FieldResult>();
        }

        public async Task<StellarDsResult<TableResult>> CreateTable(Type model, string name)
        {
            var metaData = model.GetCustomAttribute<StellarDsTable>() ?? throw new NullReferenceException($"{nameof(model)} is not decorated with a StellarDsTable attribute");

            var stellarDsResult = await CreateTable(name, metaData.Description, metaData.IsMultiTenant);

            if (stellarDsResult.Data is not { } tableResult) return stellarDsResult;

            foreach (var property in model.GetProperties())
            {
                var stellarDsType = property.GetCustomAttribute<StellarDsProperty>()?.Type ?? throw new NullReferenceException($"{nameof(property)} is not decorated with a StellarDsProperty attribute.");

                //todo: do something with the stellarDsresult
                await CreateField(tableResult.Id, property.Name, stellarDsType);
            }

            return stellarDsResult;
        }

        public async Task DeleteTable(int id)
        {
            var httpClient = await GetHttpClientAsync();

            (await httpClient.DeleteAsync($"v1/schema/table?project={apiCredentials.Project}&table={id}")).EnsureSuccessStatusCode();
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            return httpClientFactory
                .CreateClient(apiSettings.Name)
                .AddAuthorization(await tokenProvider.Get());
        }
    }
}
