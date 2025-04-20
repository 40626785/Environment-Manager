using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using EnvironmentManager.Interfaces;


namespace EnvironmentManager.Data
{
    public class DatabaseAdminDbContext : DbContext, IDatabaseAdminDataStore
    {
        public DatabaseAdminDbContext(DbContextOptions<DatabaseAdminDbContext> options)
            : base(options)
        {
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
    }
}
