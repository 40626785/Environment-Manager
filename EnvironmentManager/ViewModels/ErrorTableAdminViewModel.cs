using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using EnvironmentManager.Interfaces;


namespace EnvironmentManager.ViewModels;

public partial class ErrorTableAdminViewModel : ObservableObject

{
    private readonly IDatabaseAdminDataStore _dataStore;

    public ObservableCollection<string> Records { get; } = new();

    public string Title => "ErrorTable";
    public bool HasDateFilter => false;
    public bool HasIdFilter => true;

    [ObservableProperty] public int startId;
    [ObservableProperty] public int endId;

    public IRelayCommand ClearCommand { get; }

    public ErrorTableAdminViewModel(IDatabaseAdminDataStore dataStore)
    {
        _dataStore = dataStore;
        ClearCommand = new AsyncRelayCommand(ClearAsync);
    }

    public void LoadData()
    {
        Records.Clear();
        Records.Add($"ErrorTable row with ID range {StartId}-{EndId}");
    }

    private async Task ClearAsync()
    {
        await _dataStore.ClearTableByIdRangeAsync("ErrorTable", StartId, EndId);
    }
}
