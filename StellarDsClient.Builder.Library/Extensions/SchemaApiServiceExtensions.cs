using System.Reflection;
using StellarDsClient.Builder.Library.Attributes;
using StellarDsClient.Builder.Library.Providers;
using StellarDsClient.Builder.Library.Records;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Dto.Schema;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Builder.Library.Extensions
{
    internal static class SchemaApiServiceExtensions
    {
        private const string Description = "StellarDsClient table";

        internal static async Task<bool> ValidateDataStore(this SchemaApiService<AccessTokenProvider> schemaApiService, List<Type> models, TableSettings tableSettings)
        {
            if (!tableSettings.Validate(models))
            {
                return false;
            }

            foreach (var model in models)
            {
                if ((await schemaApiService.GetTable(tableSettings[model.Name])).Data is not { } tableResult)
                {
                    return false;
                }

                if ((await schemaApiService.GetFields(tableResult.Id)).Data is not { } fields)
                {
                    return false;
                }

                if (!fields.Validate(model))
                {
                    return false;
                }
            }

            return true;
        }

        internal static async Task<TableSettings> BuildDataStore(this SchemaApiService<AccessTokenProvider> schemaApiService, List<Type> models)
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

                Environment.Exit(0); //todo: exception or return null?
            }

            if (await schemaApiService.FindMatchingTables(tableResults, models) is { } matchingTables)
            {
                if (matchingTables.Count == models.Count)
                {
                    return matchingTables;
                }

                Console.WriteLine("One or more new tables will be created");

                foreach (var model in models.Where(m => !matchingTables.ContainsKey(m.Name)))
                {
                    var tableMetadata = await schemaApiService.BuildTable(tableResults, model);
                    matchingTables.Add(model.Name, tableMetadata.Id);
                }

                return matchingTables;
            }

            Console.WriteLine("No matching tables found. All tables will be build.");

            var newTables = new TableSettings();

            foreach (var model in models)
            {
                var tableMetadata = await schemaApiService.BuildTable(tableResults, model);
                newTables.Add(model.Name, tableMetadata.Id);
            }

            return newTables;
        }

        private static async Task<TableSettings?> FindMatchingTables(this SchemaApiService<AccessTokenProvider> schemaApiService, ICollection<TableResult> tableResults, ICollection<Type> models)
        {
            if (tableResults.Count < models.Count)
            {
                return null;
            }

            //loop over tablesResults / metedataMatches twice => prevents getting the fields for all the tables

            //todo: what's with the any? surely this can be done more efficient?
            var metadataMatches = tableResults.Where(t => models.Select(m => m.GetCustomAttribute<StellarDsTable>() ?? throw new NullReferenceException($"Model not annotated with attribute {nameof(StellarDsTable)}")).Any(s => s.IsMultiTenant == t.IsMultitenant && s.Description == t.Description)).ToList();

            if (metadataMatches.Count < models.Count)
            {
                return null;
            }

            var results = new List<TableModelMatch>();

            foreach (var metadata in metadataMatches)
            {
                var stellarDsResult = await schemaApiService.GetFields(metadata.Id);

                //todo: return null instead of throwing exception?
                var fields = stellarDsResult.Data ?? throw new NullReferenceException(string.Join("\n", stellarDsResult.Messages.Select(e => e.Message)));

                results.AddRange(
                    from model in models
                    where fields.Validate(model)
                    select new TableModelMatch(model, metadata));
            }

            var newTableSettings = new TableSettings();

            const string createNewTable = "new";

            var grouped = results.GroupBy(x => x.Model);
            foreach (var group in grouped)
            {
                Console.WriteLine($"The following tables match the {group.Key.Name} model: ");
                foreach (var match in group)
                {
                    Console.WriteLine($"\t {{id: {match.TableResult.Id} name: {match.TableResult.Name}}}");
                }

                Console.WriteLine($"Assign a table by selecting the id or enter '{createNewTable}' to create a new table: ");
                var input = Console.ReadLine();
                
                if (input?.Equals(createNewTable, StringComparison.InvariantCultureIgnoreCase) is true)
                {
                    continue;
                }

                if (!int.TryParse(input, out var id))
                {
                    throw new ArgumentException("The id is not of type integer");
                }

                newTableSettings.Add(group.Key.Name, id);
            }

            //todo: check if foreign keys match?

            return newTableSettings;
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

                Environment.Exit(0); //todo: exception
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

                Environment.Exit(0); //todo: exception
            }

            return newMetadata;
        }
    }
}