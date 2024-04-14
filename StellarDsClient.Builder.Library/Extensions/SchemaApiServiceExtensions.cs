using System.Reflection;
using StellarDsClient.Builder.Library.Attributes;
using StellarDsClient.Builder.Library.Providers;
using StellarDsClient.Builder.Library.Records;
using StellarDsClient.Dto.Schema;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Models;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class SchemaApiServiceExtensions
    {
        private const string Description = "StellarDsClient table";

        internal static async Task<TableSettingsDictionary> BuildDataStore(this SchemaApiService<AccessTokenProvider> schemaApiService, List<Type> models)
        {
            var tablesStellarDsResult = await schemaApiService.FindTables();

            var tableResults = tablesStellarDsResult.Data;
            if (tableResults is null)
            {
                Console.WriteLine("Unable to retrieve table metadata:");

                foreach (var message in tablesStellarDsResult.Messages)
                {
                    Console.WriteLine(message.Code + ": " + message.Message);
                }

                Environment.Exit(0);
            }

            if (await schemaApiService.FindMatchingTables(tableResults, models) is { } matchingTables)
            {
                return matchingTables;
            }

            Console.WriteLine("There have been no tables found that match the models");

            var newTables = new TableSettingsDictionary();

            foreach (var model in models)
            {
                var tableMetadata = await schemaApiService.BuildTable(tableResults, model);
                newTables.Add(model.Name, tableMetadata.Id);
            }

            return newTables;
        }

        private static async Task<TableSettingsDictionary?> FindMatchingTables(this SchemaApiService<AccessTokenProvider> schemaApiService, ICollection<TableResult> tableResults, ICollection<Type> models)
        {
            if (tableResults.Count < models.Count)
            {
                return null;
            }

            //loop over tablesResults / metedataMatches twice => prevents getting the fields for all the tables

            var metadataMatches = tableResults.Where(t => models.Select(m => m.GetCustomAttribute<StellarDsTable>() ?? throw new NullReferenceException($"Model not annotated with attribute {nameof(StellarDsTable)}")).Any(s => s.IsMultiTenant == t.IsMultitenant && s.Description == t.Description)).ToList();

            if (metadataMatches.Count < models.Count)
            {
                return null;
            }

            var results = new List<TableModelMatch>();
            
            foreach (var metadata in metadataMatches)
            {
                var stellarDsResult = await schemaApiService.GetFields(metadata.Id);

                var fields = stellarDsResult.Data ?? throw new NullReferenceException(string.Join("\n", stellarDsResult.Messages.Select(e => e.Message)));

                results.AddRange(from model in models where fields.Validate(model) select new TableModelMatch(model, metadata));
            }
            
            //api does not provide the tableId in the fieldResult => can't loop over models and use where to 

            var tableSettings = new TableSettingsDictionary();

            var grouped = results.GroupBy(x => x.Model);
            foreach (var group in grouped)
            {
                Console.WriteLine($"The following tables match the {group.Key.Name} model: ");
                foreach (var match in group)
                {
                    Console.WriteLine($"\t {{id: {match.TableResult.Id} name: {match.TableResult.Name}}}");
                }

                Console.WriteLine($"Assign a table by selecting the id: ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out var id))
                {
                    throw new ArgumentException("The id is not of type integer");
                }

                tableSettings.Add(group.Key.Name, id);
            }

            //todo: check if foreign keys match?

            return tableSettings;
        }

        private static async Task<TableResult> BuildTable(this SchemaApiService<AccessTokenProvider> schemaApiService, IEnumerable<TableResult> tableResults, Type model)
        {
            Console.WriteLine($"Enter a name for the {model.Name} table: ");
            var name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name) || tableResults.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new NullReferenceException("No title provided or the name is already taken.");
            }

            var settings = model.GetCustomAttribute<StellarDsTable>() ?? throw new NullReferenceException($"Model not annotated with attribute {nameof(StellarDsTable)}");

            var metadataStellarDsResult = await schemaApiService.CreateTable(name, settings.Description, settings.IsMultiTenant);
            var newMetadata = metadataStellarDsResult.Data;

            if (newMetadata is null)
            {
                Console.WriteLine($"Failed to create the {name} table: ");
                foreach (var message in metadataStellarDsResult.Messages)
                {
                    Console.WriteLine(message.Code + ": " + message.Message);
                }

                Environment.Exit(0);
            }

            foreach (var property in model.GetProperties())
            {
                var stellarDsType = property.GetCustomAttribute<StellarDsProperty>()?.Type ?? property.PropertyType.ToString();

                var fieldMetadataStellarDsResult = await schemaApiService.CreateField(newMetadata.Id, property.Name.ToLowerInvariant(), stellarDsType);

                if (fieldMetadataStellarDsResult.Data is not null)
                {
                    continue;
                }

                Console.WriteLine($"Failed to create field for the {name} table: ");
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