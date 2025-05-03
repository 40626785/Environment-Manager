using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class ErrorViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<ErrorDbContext> _dbContextFactory;
        private readonly IUserDialogService _dialogService;

        public ObservableCollection<ErrorEntry> TableData { get; } = new();

        public ICommand LoadDataCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand DeleteFilteredCommand { get; }
        public ICommand ExportToCsvCommand { get; }
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

        public ErrorViewModel(IDbContextFactory<ErrorDbContext> dbContextFactory, IUserDialogService dialogService)
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
                var data = await context.Errors
                    .OrderByDescending(e => e.ErrorDateTime)
                    .Take(100)
                    .ToListAsync();

                foreach (var error in data)
                    TableData.Add(error);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Failed to load error logs: {ex.Message}", "OK");
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
                IQueryable<ErrorEntry> query = context.Errors;

                if (int.TryParse(StartIdText, out int startId) &&
                    int.TryParse(EndIdText, out int endId) &&
                    startId <= endId)
                {
                    query = query.Where(e => e.ErrorID >= startId && e.ErrorID <= endId);
                }

                if (IsDateFilterEnabled)
                {
                    if (StartDate > EndDate)
                    {
                        await _dialogService.ShowAlert("Invalid Date Range", "Start date must be before end date.", "OK");
                        return;
                    }

                    query = query.Where(e => e.ErrorDateTime >= StartDate && e.ErrorDateTime <= EndDate);
                }

                var results = await query.OrderByDescending(e => e.ErrorDateTime).Take(100).ToListAsync();
                foreach (var error in results)
                    TableData.Add(error);

                if (!TableData.Any())
                    await _dialogService.ShowAlert("No Results", "No error logs match your filters.", "OK");
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

            bool confirm = await _dialogService.ShowConfirmation("Confirm Deletion", $"Delete {TableData.Count} error logs?", "Yes", "Cancel");
            if (!confirm) return;

            try
            {
                IsBusy = true;

                using var context = _dbContextFactory.CreateDbContext();
                context.Errors.RemoveRange(TableData);
                await context.SaveChangesAsync();

                await _dialogService.ShowAlert("Deleted", $"{TableData.Count} error logs removed.", "OK");
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
                var lines = new List<string> { "ErrorID,ErrorDateTime,ErrorMessage" };
                lines.AddRange(TableData.Select(error =>
                    $"{error.ErrorID},{error.ErrorDateTime:yyyy-MM-dd HH:mm:ss},{error.ErrorMessage?.Replace(",", " ")}"));

                var fileName = $"Errors_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
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
