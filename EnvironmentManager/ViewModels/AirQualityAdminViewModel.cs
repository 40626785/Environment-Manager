using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using System.Diagnostics;

public partial class AirQualityAdminViewModel : ObservableObject
{
    private readonly IDatabaseAdminDataStore _dataStore;

    // Now using strongly-typed model
    public ObservableCollection<AirQualityRecord> Records { get; } = new();

    [ObservableProperty] private DateTime filterDate = DateTime.Today;

    public string Title => "Air_Quality";
    public bool HasDateFilter => true;

    public IRelayCommand FilterCommand { get; }
    public IRelayCommand ClearCommand { get; }

    public AirQualityAdminViewModel(IDatabaseAdminDataStore dataStore)
    {
        _dataStore = dataStore;
        FilterCommand = new AsyncRelayCommand(LoadFilteredDataAsync);
        ClearCommand = new AsyncRelayCommand(ClearFilteredRowsAsync);
    }

    // Load all rows (unfiltered) on page load
    public async void LoadUnfilteredData()
    {
        Records.Clear();
        try
        {
            var data = await _dataStore.GetFilteredTableDataAsync("Air_Quality");

            if (data == null || data.Count == 0)
            {
                Debug.WriteLine("No data found in Air_Quality.");
                return;
            }

            foreach (var row in data)
            {
                Records.Add(MapRowToAirQualityRecord(row));
            }

            Debug.WriteLine($"Loaded {data.Count} unfiltered rows from Air_Quality.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading unfiltered data: {ex.Message}");
        }
    }

    // Load filtered data by date
    public async Task LoadFilteredDataAsync()
    {
        Records.Clear();
        try
        {
            var data = await _dataStore.GetFilteredTableDataAsync("Air_Quality", dateFilter: FilterDate);

            if (data == null || data.Count == 0)
            {
                Debug.WriteLine("No filtered data found.");
                return;
            }

            foreach (var row in data)
            {
                Records.Add(MapRowToAirQualityRecord(row));
            }

            Debug.WriteLine($"Loaded {data.Count} filtered rows from Air_Quality.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading filtered data: {ex.Message}");
        }
    }

    // Clear rows based on current filter
    public async Task ClearFilteredRowsAsync()
    {
        try
        {
            await _dataStore.ClearTableByDateAsync("Air_Quality", FilterDate);
            Debug.WriteLine($"Cleared rows for date {FilterDate:yyyy-MM-dd}.");

            await LoadFilteredDataAsync();  // Reload data after clearing
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error clearing data: {ex.Message}");
        }
    }

    // Helper to map Dictionary<string, object> to AirQualityRecord
    private AirQualityRecord MapRowToAirQualityRecord(Dictionary<string, object> row)
    {
        return new AirQualityRecord
        {
            Date = Convert.ToDateTime(row["Date"]),
            Time = row["Time"].ToString(),
            PM25 = Convert.ToDouble(row["PM25"]),
            PM10 = Convert.ToDouble(row["PM10"]),
            NO2 = Convert.ToDouble(row["NO2"]),
            CO = Convert.ToDouble(row["CO"])
        };
    }
}
