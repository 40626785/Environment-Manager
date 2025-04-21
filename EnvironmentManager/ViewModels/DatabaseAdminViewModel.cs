using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EnvironmentManager.ViewModels;

public partial class DatabaseAdminViewModel : ObservableObject
{
    public ObservableCollection<string> TableOptions { get; } = new()
    {
        "Air_Quality",
        "ErrorTable"
    };

    [ObservableProperty]
    private string selectedTable;

    public IRelayCommand NavigateToTableCommand { get; }

    public DatabaseAdminViewModel()
    {
        NavigateToTableCommand = new RelayCommand(GoToSelectedTable);
    }

    private async void GoToSelectedTable()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SelectedTable))
            {
                Debug.WriteLine($"Navigating to: TableAdminPage?table={SelectedTable}");
                await Shell.Current.GoToAsync($"TableAdminPage?table={SelectedTable}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Navigation failed: {ex.Message}");
        }
    }

}
