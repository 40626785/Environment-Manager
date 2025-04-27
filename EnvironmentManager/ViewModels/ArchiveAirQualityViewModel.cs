using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly ArchiveAirQualityDbContext _dbContext;

        public ObservableCollection<ArchiveAirQuality> TableData { get; set; } = new();
        public ICommand RowTappedCommand { get; }

        #region Sorting Properties

        public ICommand ExportToCsvCommand { get; }


        public List<string> SortOptions { get; } = new()
        {
            "ID",
            "Date",
            "Nitrogen_dioxide",
            "PM2_5_particulate_matter"
        };
        public string StartIdText { get; set; }
        public string EndIdText { get; set; }


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

        public ICommand ApplyFiltersCommand { get; }

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
            ApplyFiltersCommand = new Command(async () => await ApplyFiltersAsync());
            ApplySortCommand = new Command(async () => await ApplySortAsync());
            DeleteFilteredCommand = new Command(async () => await DeleteFilteredAsync());
            ToggleFilterVisibilityCommand = new Command(() => IsFilterVisible = !IsFilterVisible);
            ExportToCsvCommand = new Command(async () => await ExportToCsvAsync());
            RowTappedCommand = new Command<ArchiveAirQuality>(async (record) => await OnRowTapped(record));

            Task.Run(async () => await LoadDataAsync());
        }

        #region Methods
        private bool isDateFilterEnabled = false;   // Default ON
        public bool IsDateFilterEnabled
        {
            get => isDateFilterEnabled;
            set => SetProperty(ref isDateFilterEnabled, value);
        }

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
        private async Task OnRowTapped(ArchiveAirQuality record)
        {
            if (record == null) return;

            Debug.WriteLine($"Tapped Record ID: {record.Id}");

            // Store the record in the shared service
            NavigationDataStore.SelectedRecord = record;

            // Navigate without passing parameters
            await Shell.Current.GoToAsync(nameof(EditArchiveAirQualityPage));
        }


        private async Task ApplyFiltersAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                IQueryable<ArchiveAirQuality> query = _dbContext.ArchiveAirQuality;

                Debug.WriteLine($"[DEBUG] Applying Date Filter: {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}");

                // Apply Date Range Filter
                if (IsDateFilterEnabled)
                {
                    Debug.WriteLine($"[DEBUG] Applying Date Filter: {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}");
                    query = query.Where(a => a.Date >= StartDate && a.Date <= EndDate);
                }
                else
                {
                    Debug.WriteLine("[DEBUG] Skipping Date Filter.");
                }


                // Parse ID inputs
                bool hasStartId = int.TryParse(StartIdText, out var startId);
                bool hasEndId = int.TryParse(EndIdText, out var endId);

                Debug.WriteLine($"[DEBUG] StartId Parsed: {hasStartId} ({startId}), EndId Parsed: {hasEndId} ({endId})");

                if (hasStartId && hasEndId)
                {
                    Debug.WriteLine($"[DEBUG] Applying ID Filter: {startId} to {endId}");
                    query = query.Where(a => a.Id >= startId && a.Id <= endId);
                }
                else
                {
                    Debug.WriteLine("[DEBUG] Skipping ID filter due to invalid input.");
                }

                // Check how many records match before limiting
                var totalMatches = await query.CountAsync();
                Debug.WriteLine($"[DEBUG] Total records matching filters: {totalMatches}");

                query = query.OrderByDescending(a => a.Date);

                var data = await query.Take(100).ToListAsync();
                Debug.WriteLine($"[DEBUG] Records loaded after Take(100): {data.Count}");

                foreach (var item in data)
                    TableData.Add(item);

                if (!TableData.Any())
                    await Application.Current.MainPage.DisplayAlert("Info", "No records found with current filters.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Filter failed: {ex.Message}");
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