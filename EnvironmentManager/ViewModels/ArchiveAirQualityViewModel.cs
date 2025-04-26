using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class ArchiveAirQualityViewModel : BaseViewModel
    {
        private readonly ArchiveAirQualityDbContext _dbContext;

        public ObservableCollection<ArchiveAirQuality> TableData { get; set; } = new();

        #region Sorting Properties

        public ICommand ExportToCsvCommand { get; }


        public List<string> SortOptions { get; } = new()
        {
            "Date",
            "Nitrogen_dioxide",
            "PM2_5_particulate_matter"
        };

        private string selectedSortOption = "Nitrogen_dioxide";
        public string SelectedSortOption
        {
            get => selectedSortOption;
            set => SetProperty(ref selectedSortOption, value);
        }

        public List<string> SortDirections { get; } = new() { "Ascending", "Descending" };

        private string selectedSortDirection = "Descending";
        public string SelectedSortDirection
        {
            get => selectedSortDirection;
            set => SetProperty(ref selectedSortDirection, value);
        }

        public ICommand ApplySortCommand { get; }

        #endregion

        #region Date Filtering Properties

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

        public ICommand FilterByDateRangeCommand { get; }

        #endregion

        #region Toggle Filter Visibility

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

        public ICommand ToggleFilterVisibilityCommand { get; }

        #endregion

        public ICommand LoadDataCommand { get; }
        public ICommand DeleteFilteredCommand { get; }

        public ArchiveAirQualityViewModel(ArchiveAirQualityDbContext dbContext)
        {
            _dbContext = dbContext;

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            FilterByDateRangeCommand = new Command(async () => await FilterByDateRangeAsync());
            ApplySortCommand = new Command(async () => await ApplySortAsync());
            DeleteFilteredCommand = new Command(async () => await DeleteFilteredAsync());
            ToggleFilterVisibilityCommand = new Command(() => IsFilterVisible = !IsFilterVisible);
            ExportToCsvCommand = new Command(async () => await ExportToCsvAsync());


            Task.Run(async () => await LoadDataAsync());
        }

        #region Methods

        private async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                var data = await _dbContext.ArchiveAirQuality
                                            .OrderByDescending(a => a.Date)
                                            .Take(100)
                                            .ToListAsync();

                foreach (var item in data)
                    TableData.Add(item);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task FilterByDateRangeAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                var data = await _dbContext.ArchiveAirQuality
                    .Where(a => a.Date >= StartDate && a.Date <= EndDate)
                    .OrderBy(a => a.Date)
                    .ToListAsync();

                foreach (var item in data)
                    TableData.Add(item);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Filter failed: {ex.Message}", "OK");
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

                IQueryable<ArchiveAirQuality> query = _dbContext.ArchiveAirQuality;

                bool isAscending = SelectedSortDirection == "Ascending";

                query = SelectedSortOption switch
                {
                    "Nitrogen_dioxide" => isAscending
                        ? query.OrderBy(a => a.Nitrogen_dioxide)
                        : query.OrderByDescending(a => a.Nitrogen_dioxide),

                    "PM2_5_particulate_matter" => isAscending
                        ? query.OrderBy(a => a.PM2_5_particulate_matter)
                        : query.OrderByDescending(a => a.PM2_5_particulate_matter),

                    "Date" => isAscending
                        ? query.OrderBy(a => a.Date)
                        : query.OrderByDescending(a => a.Date),

                    _ => query
                };

                var data = await query.Take(100).ToListAsync();

                foreach (var item in data)
                    TableData.Add(item);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Sort failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        //export to CSV
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

                var csvLines = new List<string>();

                // Header
                csvLines.Add("Id,Date,Time,Nitrogen_dioxide,Sulphur_dioxide,PM2_5_particulate_matter,PM10_particulate_matter");

                // Data rows
                foreach (var item in TableData)
                {
                    var line = $"{item.Id}," +
                               $"{item.Date:yyyy-MM-dd}," +
                               $"{item.Time}," +
                               $"{item.Nitrogen_dioxide}," +
                               $"{item.Sulphur_dioxide}," +
                               $"{item.PM2_5_particulate_matter}," +
                               $"{item.PM10_particulate_matter}";

                    csvLines.Add(line);
                }

                // File path (adjust for platform if needed)
                var fileName = $"ArchiveAirQuality_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                await File.WriteAllLinesAsync(filePath, csvLines);

                await Application.Current.MainPage.DisplayAlert("Export Complete", $"CSV saved to:\n{filePath}", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Export failed: {ex.Message}", "OK");
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

                //  Confirmation Dialog
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Deletion",
                    $"You are about to delete {TableData.Count} records.\n\nThis action cannot be undone.\n\nAre you sure?",
                    "Yes, Delete",
                    "Cancel");

                if (!confirm)
                {
                    await Application.Current.MainPage.DisplayAlert("Cancelled", "No records were deleted.", "OK");
                    return;
                }

                // Proceed with deletion
                _dbContext.ArchiveAirQuality.RemoveRange(TableData);
                await _dbContext.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert("Success", $"{TableData.Count} records deleted.", "OK");

                await LoadDataAsync();
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

        #endregion
    }


}