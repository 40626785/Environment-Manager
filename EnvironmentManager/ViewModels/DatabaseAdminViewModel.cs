using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace EnvironmentManager.ViewModels;

public class DatabaseAdminViewModel : ObservableObject
{
    private readonly IDatabaseAdminDataStore _dataStore;

    public ObservableCollection<string> TableNames { get; } = new();
    public ICommand ClearByDateCommand { get; }
    public ICommand ClearByIdRangeCommand { get; }

    private string _selectedTable;
    public string SelectedTable
    {
        get => _selectedTable;
        set
        {
            _selectedTable = value;
            OnPropertyChanged();
        }
    }

    private DateTime _filterDate = DateTime.Today;
    public DateTime FilterDate
    {
        get => _filterDate;
        set
        {
            _filterDate = value;
            OnPropertyChanged();
        }
    }

    public int StartId { get; set; }
    public int EndId { get; set; }

    public DatabaseAdminViewModel(IDatabaseAdminDataStore dataStore)
    {
        _dataStore = dataStore;

        try
        {
            var tables = _dataStore.GetAllTableNames();
            foreach (var table in tables)
                TableNames.Add(table);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load table names: {ex.Message}");
        }

        ClearByDateCommand = new AsyncRelayCommand(ClearByDate);
        ClearByIdRangeCommand = new AsyncRelayCommand(ClearByIdRange);
    }

    private async Task ClearByDate()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SelectedTable))
                await _dataStore.ClearTableByDateAsync(SelectedTable, FilterDate);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to clear by date: {ex.Message}");
        }
    }

    private async Task ClearByIdRange()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SelectedTable))
                await _dataStore.ClearTableByIdRangeAsync(SelectedTable, StartId, EndId);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to clear by ID range: {ex.Message}");
        }
    }
}
