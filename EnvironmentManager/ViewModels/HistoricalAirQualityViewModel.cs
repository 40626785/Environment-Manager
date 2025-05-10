using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class HistoricalAirQualityViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<ArchiveAirQualityDbContext> _dbContextFactory;
        private readonly ILoggingService _logger;  // Injected logging service

        public List<string> SortOptions { get; private set; }
        // List of available years for filtering
        public List<int> AvailableYears { get; private set; }

        // List of available months for filtering
        public List<string> AvailableMonths { get; private set; }

        // Selected year and month for filtering
        private int selectedYear;
        public int SelectedYear
        {
            get => selectedYear;
            set => SetProperty(ref selectedYear, value);
        }

        private string selectedMonth;
        public string SelectedMonth
        {
            get => selectedMonth;
            set => SetProperty(ref selectedMonth, value);
        }
        public ObservableCollection<ArchiveAirQuality> AirQualityData { get; set; } = new();

        public ICommand ApplyFiltersCommand { get; }
        public ICommand ApplySortCommand { get; }
        public ICommand ExportToCsvCommand { get; }

        public string StartIdText { get; set; }
        public string EndIdText { get; set; }

        private string selectedSortOption = "Date";
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

        private bool isDateFilterEnabled = false;
        public bool IsDateFilterEnabled
        {
            get => isDateFilterEnabled;
            set => SetProperty(ref isDateFilterEnabled, value);
        }

        // Constructor that receives the DbContextFactory and ILoggingService
        public HistoricalAirQualityViewModel(
            IDbContextFactory<ArchiveAirQualityDbContext> dbContextFactory,
            ILoggingService logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;

            ApplyFiltersCommand = new Command(async () => await ApplyFilterAsync());
            ApplySortCommand = new Command(async () => await ApplySortAsync());
            ExportToCsvCommand = new Command(async () => await ExportToCsvAsync());
            LoadAvailableYears();
            LoadAvailableMonths();
            LoadSortOptions();
        }

        private async void LoadAvailableYears()
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                // Get the list of distinct years from the Date property (handle nullable DateTime?)
                AvailableYears = await context.ArchiveAirQuality
                    .Where(a => a.Date.HasValue)  // Ensure Date is not null
                    .OrderBy(a => a.Date.Value.Year)  // Access the Year from Date.Value
                    .Select(a => a.Date.Value.Year)  // Get the Year from Date
                    .Distinct()
                    .ToListAsync();

                // Add an "All" option to the list (for cases where you want to show all years)
                AvailableYears.Insert(0, 0); // 0 can represent "All"
                SelectedYear = 0;  // Set the default to "All"
            }
            catch (Exception ex)
            {
                // Handle any errors while loading the years (e.g., logging)
                Console.WriteLine($"Error loading years: {ex.Message}");
            }
        }

        private void LoadAvailableMonths()
        {
            AvailableMonths = new List<string>
            {
                "All", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
            };

            // Default to "All" month
            SelectedMonth = "All";
        }

        // Load available sort options
        private void LoadSortOptions()
        {
            SortOptions = new List<string>
            {
                "Date", "Nitrogen_dioxide", "Sulphur_dioxide", "PM2_5_particulate_matter", "PM10_particulate_matter"
            };
        }
        // Load data (for initial load or testing purposes)
        internal async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                AirQualityData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                var data = await context.ArchiveAirQuality.OrderByDescending(a => a.Date).Take(100).ToListAsync();

                foreach (var item in data)
                    AirQualityData.Add(item);

                // Log the successful load of data
                await _logger.LogMessageAsync("Data loaded successfully.");
            }
            catch (Exception ex)
            {
                // Log the error if loading fails
                await _logger.LogErrorAsync($"Error in LoadDataAsync: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Apply filters (date range and ID filters)
        public async Task ApplyFilterAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                AirQualityData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                IQueryable<ArchiveAirQuality> query = context.ArchiveAirQuality;

                if (!string.IsNullOrWhiteSpace(StartIdText) || !string.IsNullOrWhiteSpace(EndIdText))
                {
                    bool hasStartId = int.TryParse(StartIdText, out int startId);
                    bool hasEndId = int.TryParse(EndIdText, out int endId);

                    if (hasStartId && hasEndId && startId <= endId)
                    {
                        query = query.Where(a => a.Id >= startId && a.Id <= endId);
                    }
                    else
                    {
                        await _logger.LogMessageAsync("Invalid ID range.");
                        return;
                    }
                }

                var data = await query.OrderByDescending(a => a.Date).Take(100).ToListAsync();

                foreach (var item in data)
                    AirQualityData.Add(item);

                // Log the filtered data count
                await _logger.LogMessageAsync($"Applied filters. {AirQualityData.Count} records found.");
            }
            catch (Exception ex)
            {
                // Log error if filtering fails
                await _logger.LogErrorAsync($"Error in ApplyFilterAsync: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Apply sorting based on the selected options
        public async Task ApplySortAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                AirQualityData.Clear();

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
                    AirQualityData.Add(item);

                // Log the successful sorting
                await _logger.LogMessageAsync("Sorting applied successfully.");
            }
            catch (Exception ex)
            {
                // Log the error if sorting fails
                await _logger.LogErrorAsync($"Error in ApplySortAsync: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Export the filtered and sorted data to CSV
        public async Task ExportToCsvAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (!AirQualityData.Any())
                {
                    await _logger.LogMessageAsync("No data to export.");
                    return;
                }

                var csvLines = new List<string>
                {
                    "Id,Date,Time,Nitrogen_dioxide,Sulphur_dioxide,PM2_5_particulate_matter,PM10_particulate_matter"
                };

                foreach (var item in AirQualityData)
                {
                    csvLines.Add($"{item.Id},{item.Date:yyyy-MM-dd},{item.Time},{item.Nitrogen_dioxide},{item.Sulphur_dioxide},{item.PM2_5_particulate_matter},{item.PM10_particulate_matter},{item.LocationId}");
                }

                var fileName = $"HistoricalAirQuality_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                await File.WriteAllLinesAsync(filePath, csvLines);

                // Log the file path of the exported CSV
                await _logger.LogMessageAsync($"CSV Exported to {filePath}");
            }
            catch (Exception ex)
            {
                // Log the error if CSV export fails
                await _logger.LogErrorAsync($"Error in ExportToCsvAsync: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}