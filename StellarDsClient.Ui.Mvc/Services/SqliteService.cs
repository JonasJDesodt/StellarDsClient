using Microsoft.Data.Sqlite;
using StellarDsClient.Sdk.Settings;

namespace StellarDsClient.Ui.Mvc.Services
{
    public class SqliteService(IConfiguration configuration)
    {
        private readonly string? _connectionString = configuration.GetConnectionString("SqliteConnection");

        //todo: try/catch
        public void EnsureDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            // Use a transaction to handle the setup as an atomic operation
            using var transaction = connection.BeginTransaction();

            var command = connection.CreateCommand();
            command.Transaction = transaction;

            // Create the TableSettings table with 'Key' as a UNIQUE column
            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS TableSettings (
                    Key TEXT PRIMARY KEY,
                    Identity INTEGER NOT NULL
                );
            ";

            //@"
            //CREATE TABLE IF NOT EXISTS TableSettings (
            //    Id INTEGER PRIMARY KEY AUTOINCREMENT,
            //    Key TEXT NOT NULL UNIQUE,
            //    Identity INTEGER NOT NULL
            //);
            //";
            command.ExecuteNonQuery();

            // Insert initial records if they do not exist
            command.CommandText =
            @"
                    INSERT INTO TableSettings (Key, Identity)
                    SELECT 'List', 0
                    WHERE NOT EXISTS (SELECT 1 FROM TableSettings WHERE Key = 'List');
                    ";
            command.ExecuteNonQuery();

            // Insert record with key 'ToDo'
            command.CommandText =
            @"
                    INSERT INTO TableSettings (Key, Identity)
                    SELECT 'ToDo', 0
                    WHERE NOT EXISTS (SELECT 1 FROM TableSettings WHERE Key = 'ToDo');
                    ";
            command.ExecuteNonQuery();

            // Commit the transaction to apply the changes
            transaction.Commit();
        }


        //todo: try/catch
        public void UpdateTableSettings(int listTableId, int toDoTableId)
        {
            if(listTableId == toDoTableId)
            {
                return;
            }

            if(listTableId < 0 ||  toDoTableId < 0)
            {
                return;
            }

            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            var command = connection.CreateCommand();
            command.Transaction = transaction;

            // Prepare the UPDATE statement to update the identity where the id matches
            command.CommandText = @"
                UPDATE TableSettings SET Identity = @listTableId WHERE Key = 'List';
                UPDATE TableSettings SET Identity = @toDoTableId WHERE Key = 'ToDo';
            ";
            command.Parameters.AddWithValue("@listTableId", listTableId);
            command.Parameters.AddWithValue("@toDoTableId", toDoTableId);

            // Execute the command
            command.ExecuteNonQuery();

            // Commit the transaction
            transaction.Commit();
        }

        //todo: try/catch
        public int? GetTableId(string key)
        {
            int? id = null;

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Identity FROM TableSettings WHERE Key = @key";
                command.Parameters.AddWithValue("@key", key);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    id = Convert.ToInt32(reader["Identity"]);
                }
            }

            return id;
        }
    }
}
