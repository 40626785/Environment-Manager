using System;

namespace EnvironmentManager.Interfaces;

public interface IDatabaseAdminDataStore
{
    List<string> GetAllTableNames();
    Task ClearTableByDateAsync(string tableName, DateTime date);
    Task ClearTableByIdRangeAsync(string tableName, int startId, int endId);
    Task<List<Dictionary<string, object>>> GetFilteredTableDataAsync(
        string tableName,
        DateTime? dateFilter = null,
        int? startId = null,
        int? endId = null);


}
