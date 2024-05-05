using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Extensions;
using System.Net.Http.Json;
using System.Reflection;
using StellarDsClient.Sdk.Attributes;
using StellarDsClient.Sdk.Dto.Schema;
using StellarDsClient.Sdk.Dto.Transfer;
using System.Collections.Generic;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Sdk
{
    public class SchemaApiService<TTokenProvider>(IHttpClientFactory httpClientFactory, TTokenProvider tokenProvider, StellarDsClientSettings stellarDsClientSettings) where TTokenProvider : ITokenProvider
    {
        public async Task<StellarDsResult<IList<TableResult>>> FindTables()
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.GetAsync($"v1/schema/table?project={stellarDsClientSettings.ApiSettings.Project}");

            return await httpResponseMessage.ToStellarDsResult<IList<TableResult>>();
        }

        public async Task<StellarDsResult<TableResult>> GetTable(int id)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.GetAsync($"v1/schema/table?project={stellarDsClientSettings.ApiSettings.Project}&table={id}");

            return await httpResponseMessage.ToStellarDsResult<TableResult>();
        }

        //todo: create request
        public async Task<StellarDsResult<TableResult>> CreateTable(string title, string? description, bool isMultiTenant)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.PostAsJsonAsync($"v1/schema/table?project={stellarDsClientSettings.ApiSettings.Project}", new { name = title, description, isMultitenant = isMultiTenant });

            return await httpResponseMessage.ToStellarDsResult<TableResult>();
        }

        public async Task<StellarDsResult<IList<FieldResult>>> GetFields(int tableId)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.GetAsync($"v1/schema/table/field?project={stellarDsClientSettings.ApiSettings.Project}&table={tableId}");

            return await httpResponseMessage.ToStellarDsResult<IList<FieldResult>>();
        }

        public async Task<StellarDsResult<FieldResult>> CreateField(int tableId, string title, string stellarDsType)
        {
            var httpClient = await GetHttpClientAsync();

            var httpResponseMessage = await httpClient.PostAsJsonAsync($"v1/schema/table/field?project={stellarDsClientSettings.ApiSettings.Project}&table={tableId}", new { name = title, type = stellarDsType });

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

            (await httpClient.DeleteAsync($"v1/schema/table?project={stellarDsClientSettings.ApiSettings.Project}&table={id}")).EnsureSuccessStatusCode();
        }

        public async Task SetTableSettings(Type[] models)
        {
            var stellarDsResult = await FindTables();
            if (stellarDsResult?.Data is not { } tables)
            {
                return;
            }

            var tableResults = new List<TableResult>();

            if (tables.Count == 0)
            {
                foreach (var model in models)
                {
                    if ((await CreateTable(model, stellarDsClientSettings.ApiSettings.Tables[model.Name].Name))?.Data is { } tableResult)
                    {
                        tableResults.Add(tableResult);

                        stellarDsClientSettings.ApiSettings.Tables[model.Name].Name = tableResult.Name;
                        stellarDsClientSettings.ApiSettings.Tables[model.Name].Id = tableResult.Id;
                    }
                }
            }
            else
            {
                foreach(var model in models)
                {
                    if(tables.Any(x => x.Name == model.Name))
                    {
                        return;
                    }

                    if (tables.FirstOrDefault(t => t.Name.Equals(stellarDsClientSettings.ApiSettings.Tables[model.Name].Name)) is not { } existingTableResult)
                    {
                        if ((await CreateTable(model, stellarDsClientSettings.ApiSettings.Tables[model.Name].Name))?.Data is { } tableResult)
                        {
                            stellarDsClientSettings.ApiSettings.Tables[model.Name].Name = tableResult.Name;
                            stellarDsClientSettings.ApiSettings.Tables[model.Name].Id = tableResult.Id;
                        }
                    }
                    else
                    {
                        if ((await GetFields(existingTableResult.Id)).Data is { } existingListTableFields && existingTableResult.IsValid(existingListTableFields, model))
                        {
                            stellarDsClientSettings.ApiSettings.Tables[model.Name].Name = existingTableResult.Name;
                            stellarDsClientSettings.ApiSettings.Tables[model.Name].Id = existingTableResult.Id;
                        }
                    }
                }
            }
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            return httpClientFactory
                .CreateClient(stellarDsClientSettings.ApiSettings.Name)
                .AddAuthorization(await tokenProvider.Get());
        }
    }
}
