using Microsoft.AspNetCore.Http;
using StellarDsClient.Dto.Schema;
using StellarDsClient.Generator.Attributes;
using StellarDsClient.Generator.Models;
using StellarDsClient.Generator.Providers;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StellarDsClient.Generator.Extensions
{
    internal static class SchemaApiServiceExtensions
    {
        internal static async Task<TableSettings> BuildDatabase(this SchemaApiService<AccessTokenProvider> schemaApiService)
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

            var newListMetadata = await schemaApiService.BuildTable<ListPropertyMapper>("list", tableResults);
            var newTaskMetadata = await schemaApiService.BuildTable<TaskPropertyMapper>("task", tableResults);

            return new TableSettings
            {
                ListTableId = newListMetadata.Id,
                TaskTableId = newTaskMetadata.Id
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
