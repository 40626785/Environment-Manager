using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.Services;
using EnvironmentManager.Views;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class ArchiveAirQualityViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<ArchiveAirQualityDbContext> _dbContextFactory;
        private readonly DatabaseLoggingService _logger;

        public ObservableCollection<ArchiveAirQuality> TableData { get; set; } = new();
        public ICommand RowTappedCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand ApplySortCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ToggleFilterVisibilityCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand DeleteFilteredCommand { get; }

        public List<string> SortOptions { get; } = new() { "ID", "Date", "Nitrogen_dioxide", "PM2_5_particulate_matter" };
        public List<string> SortDirections { get; } = new() { "Ascending", "Descending" };

        public string StartIdText { get; set; }
        public string EndIdText { get; set; }

        private string selectedSortOption = "Nitrogen_dioxide";
        public string SelectedSortOption
        {
            get => selectedSortOption;
            set => SetProperty(ref selectedSortOption, value);
        }

        private string selectedSortDirection = "Descending";
        public string SelectedSortDirection
        {
            get => selectedSortDirection;
            set => SetProperty(ref selectedSortDirection, value);
        }

        private DateTime startDate = DateTime.Now.AddDays(-7);
        public DateTime StartDate
        {
            get => startDate;
            set => SetProperty(ref startDate, value);
        }

        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => endDate;
            set => SetProperty(ref endDate, value);
        }

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

        public string ToggleFilterText => IsFilterVisible ? "Hide Filters ▲" : "Show Filters & Export Options ▼";

        private bool isDateFilterEnabled = false;
        public bool IsDateFilterEnabled
        {
            get => isDateFilterEnabled;
            set => SetProperty(ref isDateFilterEnabled, value);
        }

        public ArchiveAirQualityViewModel(IDbContextFactory<ArchiveAirQualityDbContext> dbContextFactory, DatabaseLoggingService logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            ApplyFiltersCommand = new Command(async () => await ApplyFiltersAsync());
            ApplySortCommand = new Command(async () => await ApplySortAsync());
            DeleteFilteredCommand = new Command(async () => await DeleteFilteredAsync());
            ToggleFilterVisibilityCommand = new Command(() => IsFilterVisible = !IsFilterVisible);
            ExportToCsvCommand = new Command(async () => await ExportToCsvAsync());
            RowTappedCommand = new Command<ArchiveAirQuality>(async (record) => await OnRowTapped(record));

            Task.Run(async () => await LoadDataAsync());
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                var data = await context.ArchiveAirQuality.OrderByDescending(a => a.Date).Take(100).ToListAsync();

                foreach (var item in data)
                    TableData.Add(item);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
                await _logger.LogErrorAsync($"LoadDataAsync Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ApplyFiltersAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                IQueryable<ArchiveAirQuality> query = context.ArchiveAirQuality;

                if (IsDateFilterEnabled)
                {
                    if (StartDate > EndDate)
                    {
                        await Application.Current.MainPage.DisplayAlert("Invalid Date Range", "Start Date cannot be after End Date.", "OK");
                        return;
                    }
                    query = query.Where(a => a.Date >= StartDate && a.Date <= EndDate);
                }

                bool hasStartId = int.TryParse(StartIdText, out int startId);
                bool hasEndId = int.TryParse(EndIdText, out int endId);

                if (!string.IsNullOrWhiteSpace(StartIdText) || !string.IsNullOrWhiteSpace(EndIdText))
                {
                    if (!hasStartId || !hasEndId || startId > endId)
                    {
                        await Application.Current.MainPage.DisplayAlert("Invalid IDs", "Check Start ID and End ID inputs.", "OK");
                        return;
                    }

                    query = query.Where(a => a.Id >= startId && a.Id <= endId);
                }

                var data = await query.OrderByDescending(a => a.Date).Take(100).ToListAsync();

                foreach (var item in data)
                    TableData.Add(item);

                if (!TableData.Any())
                    await Application.Current.MainPage.DisplayAlert("Info", "No records found with current filters.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Filter failed: {ex.Message}", "OK");
                await _logger.LogErrorAsync($"ApplyFiltersAsync Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ApplySortAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                IQueryable<ArchiveAirQuality> query = context.ArchiveAirQuality;

                bool isAscending = SelectedSortDirection == "Ascending";

                query = SelectedSortOption switch
                {
                    "Nitrogen_dioxide" => isAscending ? query.OrderBy(a => a.Nitrogen_dioxide) : query.OrderByDescending(a => a.Nitrogen_dioxide),
                    "PM2_5_particulate_matter" => isAscending ? query.OrderBy(a => a.PM2_5_particulate_matter) : query.OrderByDescending(a => a.PM2_5_particulate_matter),
                    "Date" => isAscending ? query.OrderBy(a => a.Date) : query.OrderByDescending(a => a.Date),
                    "ID" => isAscending ? query.OrderBy(a => a.Id) : query.OrderByDescending(a => a.Id),
                    _ => query
                };

                var data = await query.Take(100).ToListAsync();

                foreach (var item in data)
                    TableData.Add(item);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Sort failed: {ex.Message}", "OK");
                await _logger.LogErrorAsync($"ApplySortAsync Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExportToCsvAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (!TableData.Any())
                {
                    await Application.Current.MainPage.DisplayAlert("Info", "No data to export.", "OK");
                    return;
                }

                var csvLines = new List<string>
                {
                    "Id,Date,Time,Nitrogen_dioxide,Sulphur_dioxide,PM2_5_particulate_matter,PM10_particulate_matter"
                };

                foreach (var item in TableData)
                {
                    csvLines.Add($"{item.Id},{item.Date:yyyy-MM-dd},{item.Time},{item.Nitrogen_dioxide},{item.Sulphur_dioxide},{item.PM2_5_particulate_matter},{item.PM10_particulate_matter}");
                }

                var fileName = $"ArchiveAirQuality_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                await File.WriteAllLinesAsync(filePath, csvLines);

                await Application.Current.MainPage.DisplayAlert("Export Complete", $"CSV saved to:\n{filePath}", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Export failed: {ex.Message}", "OK");
                await _logger.LogErrorAsync($"ExportToCsvAsync Error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteFilteredAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (!TableData.Any())
                {
                    await Application.Current.MainPage.DisplayAlert("Info", "No records to delete.", "OK");
                    return;
                }

                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Deletion",
                    $"You are about to delete {TableData.Count} records. This action cannot be undone.",
                    "Yes", "Cancel");

                if (!confirm) return;

                using var context = _dbContextFactory.CreateDbContext();

                context.ArchiveAirQuality.RemoveRange(TableData);
                await context.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert("Success", $"{TableData.Count} records deleted.", "OK");

                // Reload with fresh context
                await ReloadDataWithNewContextAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Delete failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ReloadDataWithNewContextAsync()
        {
            try
            {
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();

                var data = await context.ArchiveAirQuality
                                        .OrderByDescending(a => a.Date)
                                        .Take(100)
                                        .ToListAsync();

                foreach (var item in data)
                    TableData.Add(item);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Reload failed: {ex.Message}", "OK");
            }
        }


        private async Task OnRowTapped(ArchiveAirQuality record)
        {
            if (record == null) return;

            NavigationDataStore.SelectedRecord = record;
            await Shell.Current.GoToAsync(nameof(EditArchiveAirQualityPage));
        }
    }
}
