using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.Views;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels;

public partial class AlertViewModel : ObservableObject
{
    private readonly IDbContextFactory<AlertDbContext> _dbContextFactory;

    public ObservableCollection<Alert> ActiveAlerts { get; set; } = new();

    public AlertViewModel(IDbContextFactory<AlertDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        Task.Run(async () => await LoadActiveAlerts());
    }

    [RelayCommand]
    private async Task LoadActiveAlerts()
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var alerts = await dbContext.AlertTable
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.Date_Time)
                .ToListAsync();

            ActiveAlerts.Clear();
            foreach (var alert in alerts)
            {
                ActiveAlerts.Add(alert);
            }

            if (!alerts.Any())
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] No active alerts found.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {alerts.Count} active alerts.");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load active alerts: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task MarkAsResolved(int alertId)
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var alert = await dbContext.AlertTable.FindAsync(alertId);
            if (alert != null)
            {
                alert.IsResolved = true;
                await dbContext.SaveChangesAsync();
                await LoadActiveAlerts();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to mark alert as resolved: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ViewAllResolvedAlerts()
    {
        await Shell.Current.GoToAsync(nameof(ResolvedAlertsPage));
    }
}
