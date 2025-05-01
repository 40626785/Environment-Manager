using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<LogDbContext> _dbContextFactory;
        private readonly IUserDialogService _dialogService;

        public ObservableCollection<LogEntry> TableData { get; } = new();

        public ICommand LoadDataCommand { get; }
        public ICommand DeleteFilteredCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ToggleFilterVisibilityCommand { get; }

        public string StartIdText { get; set; } = string.Empty;
        public string EndIdText { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public bool IsDateFilterEnabled { get; set; } = false;

        private bool isFilterVisible = false;
        public bool IsFilterVisible
        {
            get => isFilterVisible;
            set
            {
                SetProperty(ref isFilterVisible, value);
                OnPropertyChanged(nameof(ToggleFilterText));
            }
        }

        public string ToggleFilterText => IsFilterVisible ? "Hide Filters ▲" : "Show Filters ▼";

        public LogViewModel(IDbContextFactory<LogDbContext> dbContextFactory, IUserDialogService dialogService)
        {
            _dbContextFactory = dbContextFactory;
            _dialogService = dialogService;

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            ApplyFiltersCommand = new Command(async () => await ApplyFiltersAsync());
            DeleteFilteredCommand = new Command(async () => await DeleteFilteredAsync());
            ExportToCsvCommand = new Command(async () => await ExportToCsvAsync());
            ToggleFilterVisibilityCommand = new Command(() => IsFilterVisible = !IsFilterVisible);
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                var data = await context.Logs
                    .OrderByDescending(l => l.LogDateTime)
                    .Take(100)
                    .ToListAsync();

                foreach (var log in data)
                    TableData.Add(log);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Failed to load logs: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        internal async Task ApplyFiltersAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                IQueryable<LogEntry> query = context.Logs;

                // ID Range
                if (int.TryParse(StartIdText, out int startId) &&
                    int.TryParse(EndIdText, out int endId) &&
                    startId <= endId)
                {
                    query = query.Where(l => l.LogID >= startId && l.LogID <= endId);
                }

                // Date Range
                if (IsDateFilterEnabled)
                {
                    if (StartDate > EndDate)
                    {
                        await _dialogService.ShowAlert("Invalid Date Range", "Start Date cannot be after End Date.", "OK");
                        return;
                    }

                    query = query.Where(l => l.LogDateTime >= StartDate && l.LogDateTime <= EndDate);
                }

                var results = await query.OrderByDescending(l => l.LogDateTime).Take(100).ToListAsync();
                foreach (var log in results)
                    TableData.Add(log);

                if (!TableData.Any())
                    await _dialogService.ShowAlert("No Results", "No log entries match your filters.", "OK");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Filter failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        internal async Task DeleteFilteredAsync()
        {
            if (IsBusy || !TableData.Any()) return;

            bool confirm = await _dialogService.ShowConfirmation("Confirm Deletion", $"Delete {TableData.Count} log entries?", "Yes", "Cancel");
            if (!confirm) return;

            try
            {
                IsBusy = true;

                using var context = _dbContextFactory.CreateDbContext();
                context.Logs.RemoveRange(TableData);
                await context.SaveChangesAsync();

                await _dialogService.ShowAlert("Deleted", $"{TableData.Count} log entries removed.", "OK");
                TableData.Clear();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Delete failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExportToCsvAsync(string outputPath = null)
        {
            if (!TableData.Any())
            {
                await _dialogService.ShowAlert("No Data", "There is no data to export.", "OK");
                return;
            }

            try
            {
                var lines = new List<string> { "LogID,LogDateTime,LogMessage" };
                lines.AddRange(TableData.Select(log =>
                    $"{log.LogID},{log.LogDateTime:yyyy-MM-dd HH:mm:ss},{log.LogMessage?.Replace(",", " ")}"));

                var fileName = $"Logs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = outputPath ?? Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                await File.WriteAllLinesAsync(filePath, lines);
                await _dialogService.ShowAlert("Exported", $"CSV saved to:\n{filePath}", "OK");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Export failed: {ex.Message}", "OK");
            }
        }


    }
}
