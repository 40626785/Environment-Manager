using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.Views;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels;

/// <summary>
/// ViewModel for the home page dashboard.
/// Currently displays static demo data until full implementation.
/// </summary>
public partial class HomeViewModel : ObservableObject
{
    // Basic properties for demo data display
    [ObservableProperty]
    private string _airQualityValue = "72";

    [ObservableProperty]
    private string _airQualityStatus = "Moderate";

    [ObservableProperty]
    private string _waterPhValue = "7.2";

    [ObservableProperty]
    private string _waterPhStatus = "Normal";

    [ObservableProperty]
    private string _temperatureValue = "24°C";

    [ObservableProperty]
    private string _weatherStatus = "Partly Cloudy";

    [ObservableProperty]
    private string _sensorCountValue = "42";

    [ObservableProperty]
    private string _sensorWarningCount = "7";
    private readonly IDbContextFactory<AlertDbContext> _dbContextFactory;

    [ObservableProperty]
    private ObservableCollection<Alert> topAlerts = new();

    public HomeViewModel(IDbContextFactory<AlertDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        Task.Run(async () => await LoadTopAlerts());
    }

    [RelayCommand]
    private async Task LoadTopAlerts()
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var alerts = await dbContext.AlertTable
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.Date_Time)
                .Take(2)  // Fetch top 2 alerts
                .ToListAsync();

            TopAlerts.Clear();
            foreach (var alert in alerts)
            {
                TopAlerts.Add(alert);
            }
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {alerts.Count} top active alerts.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load top alerts: {ex.Message}");
        }
    }
    public HomeViewModel()
    {
        // Simple constructor with no dependencies
        // When the real implementation is needed, this can be expanded
    }
    [RelayCommand]
    private async Task NavigateToAlertsAsync()
    {
        await Shell.Current.GoToAsync(nameof(AlertPage));
    }

    [RelayCommand]
    private void RefreshDashboard()
    {
        // In the future, this will load real data
        // For now, it's just a placeholder for the UI interaction
    }
}
