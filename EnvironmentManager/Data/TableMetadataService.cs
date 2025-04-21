using System;

namespace EnvironmentManager.Data;

public class TableMetadataService
{
    private readonly Dictionary<string, string> tableDateColumns = new()
    {
        { "Air_Quality", "Date" },
        { "Archive_Air_Quality", "Date" },
        { "Water_Quality", "Date" },
        { "Archive_Water_Quality", "Date" },
        { "archive_weather_data", "Date" },
        { "weather_data", "Date" },
        { "Maintenance", "DueDate" }
    };

    public bool TryGetDateColumn(string tableName, out string columnName)
    {
        return tableDateColumns.TryGetValue(tableName, out columnName);
    }

    public bool TableHasIdColumn(string tableName)
    {
        // All your tables seem to have ID-based access
        return true;
    }
}
