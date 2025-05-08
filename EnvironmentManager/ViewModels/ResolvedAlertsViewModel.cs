using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels;

public partial class ResolvedAlertsViewModel : ObservableObject
{
    private readonly IDbContextFactory<AlertDbContext> _dbContextFactory;

    public ObservableCollection<Alert> ResolvedAlerts { get; set; } = new();

    public ResolvedAlertsViewModel(IDbContextFactory<AlertDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        Task.Run(async () => await LoadResolvedAlerts());
    }

    [RelayCommand]
    private async Task LoadResolvedAlerts()
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var alerts = await dbContext.AlertTable
                .Where(a => a.IsResolved)
                .OrderByDescending(a => a.Date_Time)
                .ToListAsync();

            ResolvedAlerts.Clear();
            foreach (var alert in alerts)
            {
                ResolvedAlerts.Add(alert);
            }

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Loaded {alerts.Count} resolved alerts.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load resolved alerts: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task DeleteResolvedAlert(int alertId)
    {
        try
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var alert = await dbContext.AlertTable.FindAsync(alertId);
            if (alert != null)
            {
                dbContext.AlertTable.Remove(alert);
                await dbContext.SaveChangesAsync();
                await LoadResolvedAlerts();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to delete resolved alert: {ex.Message}");
        }
    }
}
