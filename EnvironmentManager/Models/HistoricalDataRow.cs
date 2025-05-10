namespace EnvironmentManager.Models;

public class HistoricalDataRow
{
    public Dictionary<string, object> Data { get; set; } = new();

    public object this[string key]
    {
        get => Data.ContainsKey(key) ? Data[key] : null;
        set => Data[key] = value;
    }

    public bool ContainsDateAfter(DateTime afterDate)
    {
        var possibleDateKeys = new[] { "Date", "Date_Time" };

        foreach (var key in possibleDateKeys)
        {
            if (Data.TryGetValue(key, out var value) && DateTime.TryParse(value?.ToString(), out var rowDate))
            {
                return rowDate > afterDate;
            }
        }

        return false;
    }
}