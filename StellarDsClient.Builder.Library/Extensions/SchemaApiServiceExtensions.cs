﻿using System.Reflection;
using StellarDsClient.Builder.Library.Attributes;
using StellarDsClient.Builder.Library.Models;
using StellarDsClient.Builder.Library.Providers;
using StellarDsClient.Dto.Schema;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Models;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class SchemaApiServiceExtensions
    {
        internal static async Task<TableSettingsDictionary> BuildDatabase(this SchemaApiService<AccessTokenProvider> schemaApiService)
        {
            var tablesStellarDsResult = await schemaApiService.FindTables();
            var tableResults = tablesStellarDsResult.Data;
            if (tableResults is null) //todo: what if there are no tables? will there be an empty list or not? 
            {
                Console.WriteLine("Unable to retrieve table metadata:");

                foreach (var message in tablesStellarDsResult.Messages)
                {
                    Console.WriteLine(message.Code + ": " + message.Message);
                }

                Environment.Exit(0);
            }

            var newListMetadata = await schemaApiService.BuildTable<List>(nameof(List), tableResults);
            var newTaskMetadata = await schemaApiService.BuildTable<ToDo>(nameof(ToDo), tableResults);

            return new TableSettingsDictionary
            {
                {nameof(List), newListMetadata.Id},
                {nameof(ToDo), newTaskMetadata.Id}
            };
        }

        private static async Task<TableResult> BuildTable<TPropertyMapper>(this SchemaApiService<AccessTokenProvider> schemaApiService, string title, IList<TableResult> tableResults) where TPropertyMapper : class
        {
            if (tableResults.GetMetadata(title) is { } oldMetadata)
            {
                await schemaApiService.DeleteTable(oldMetadata.Id);
            }

            var metadataStellarDsResult = await schemaApiService.CreateTable(title, true);
            var newMetadata = metadataStellarDsResult.Data;

            if (newMetadata is null)
            {
                Console.WriteLine($"Failed to create the {title} table: ");
                foreach (var message in metadataStellarDsResult.Messages)
                {
                    Console.WriteLine(message.Code + ": " + message.Message);
                }

                Environment.Exit(0);
            }

            foreach (var property in typeof(TPropertyMapper).GetProperties())
            {
                var stellarDsType = property.GetCustomAttribute<StellarDsType>()?.Name ??
                                    property.PropertyType.ToString();

                var fieldMetadataStellarDsResult =
                    await schemaApiService.CreateField(newMetadata.Id, property.Name, stellarDsType);

                if (fieldMetadataStellarDsResult.Data is not null)
                {
                    continue;
                }

                Console.WriteLine($"Failed to create field for the {title} table: ");
                foreach (var message in fieldMetadataStellarDsResult.Messages)
                {
                    Console.WriteLine(message.Code + ": " + message.Message);
                }

                Environment.Exit(0);
            }

            return newMetadata;
        }
    }
}