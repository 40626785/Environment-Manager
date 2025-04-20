using System;

namespace EnvironmentManager.Interfaces;

public interface IDatabaseAdminDataStore
{
    List<string> GetAllTableNames();
    Task ClearTableByDateAsync(string tableName, DateTime date);
    Task ClearTableByIdRangeAsync(string tableName, int startId, int endId);
}
