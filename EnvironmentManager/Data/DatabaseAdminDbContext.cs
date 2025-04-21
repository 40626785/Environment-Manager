using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Interfaces;
using System.Data.Common;
using System.Diagnostics;

namespace EnvironmentManager.Data
{
    public class DatabaseAdminDbContext : DbContext, IDatabaseAdminDataStore
    {
        private readonly TableMetadataService _metadata;

        public DatabaseAdminDbContext() { }

        public DatabaseAdminDbContext(DbContextOptions<DatabaseAdminDbContext> options, TableMetadataService metadata)
            : base(options)
        {
            _metadata = metadata;
        }

        public List<string> GetAllTableNames()
        {
            using var connection = Database.GetDbConnection();
            connection.Open();

            var tableNames = new List<string>();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tableNames.Add(reader.GetString(0));
            }

            return tableNames;
        }


        public async Task ClearTableByDateAsync(string tableName, DateTime date)
        {
            var formattedDate = date.ToString("yyyy-MM-dd");
            var sql = $"DELETE FROM [{tableName}] WHERE CAST(Date_Time AS DATE) = '{formattedDate}'";
            await Database.ExecuteSqlRawAsync(sql);
        }

        public async Task ClearTableByIdRangeAsync(string tableName, int startId, int endId)
        {
            var sql = $"DELETE FROM [{tableName}] WHERE Id BETWEEN {startId} AND {endId}";
            await Database.ExecuteSqlRawAsync(sql);
        }

        public async Task<List<Dictionary<string, object>>> GetFilteredTableDataAsync(
            string tableName, DateTime? dateFilter = null, int? startId = null, int? endId = null)
        {
            try
            {
                using var connection = Database.GetDbConnection();
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                var whereClauses = new List<string>();

                if (dateFilter != null && _metadata.TryGetDateColumn(tableName, out var dateColumn))
                {
                    whereClauses.Add($"CAST([{dateColumn}] AS DATE) = '{dateFilter.Value:yyyy-MM-dd}'");
                }

                if (startId != null && endId != null)
                {
                    whereClauses.Add($"Id BETWEEN {startId} AND {endId}");
                }

                var where = whereClauses.Count > 0 ? $"WHERE {string.Join(" AND ", whereClauses)}" : "";
                var sql = $"SELECT TOP 50 * FROM [{tableName}] {where}";

                Debug.WriteLine($"SQL: {sql}");

                command.CommandText = sql;

                using var reader = await command.ExecuteReaderAsync();

                var results = new List<Dictionary<string, object>>();
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    results.Add(row);
                }

                return results;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetFilteredTableDataAsync failed: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }


    }
}

