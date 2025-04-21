using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using EnvironmentManager.Interfaces;
using System.Diagnostics;

public partial class AirQualityAdminViewModel : ObservableObject
{
    private readonly IDatabaseAdminDataStore _dataStore;

    public ObservableCollection<string> Records { get; } = new();

    public string Title => "Air_Quality";
    public bool HasDateFilter => true;
    public bool HasIdFilter => false;

    [ObservableProperty] private DateTime filterDate = DateTime.Today;

    public IRelayCommand ClearCommand { get; }

    public AirQualityAdminViewModel(IDatabaseAdminDataStore dataStore)
    {
        _dataStore = dataStore;
        ClearCommand = new AsyncRelayCommand(ClearAsync);
    }


    public async void LoadData()
    {
        Records.Clear();
        try
        {
            var data = await _dataStore.GetFilteredTableDataAsync("Air_Quality", dateFilter: FilterDate);

            if (data == null || data.Count == 0)
            {
                Debug.WriteLine("No rows returned from Air_Quality");
                Records.Add("No data found.");
                return;
            }

            foreach (var row in data)
            {
                Records.Add(string.Join(" | ", row.Select(kv => $"{kv.Key}: {kv.Value}")));
            }

            Debug.WriteLine($"Loaded {data.Count} rows from Air_Quality.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in LoadData: {ex.Message}");
            Records.Add(" Failed to load data.");
        }
    }




    private async Task ClearAsync()
    {
        await _dataStore.ClearTableByDateAsync("Air_Quality", FilterDate);
    }
}
