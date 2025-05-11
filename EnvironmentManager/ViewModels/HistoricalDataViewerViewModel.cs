using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;


namespace EnvironmentManager.ViewModels
{
    /// <summary>
    /// Represents the HistoricalDataViewerViewModel database context.
    /// </summary>
    public class HistoricalDataViewerViewModel : BaseViewModel
    {
        private readonly HistoricalDataDbContext _dbContext;

        public ObservableCollection<ArchiveAirQuality> AirQualityData { get; } = new();
        // Dropdown values for year and month filters

        /// <summary>
        /// Asynchronously executes the 10).ToList operation.
        /// </summary>
        public List<int> AvailableYears { get; } = Enumerable.Range(2020, 10).ToList();

        /// <summary>
        /// Asynchronously executes the = operation.
        /// </summary>
        public List<string> AvailableMonths { get; } =
            new List<string> { "All", "January", "February", "March", "April", "May", "June", "July",
                       "August", "September", "October", "November", "December" };

        private int selectedYear = DateTime.Today.Year;
        public int SelectedYear
        {
            get => selectedYear;
            set => SetProperty(ref selectedYear, value);
        }

        private string selectedMonth = "All";
        public string SelectedMonth
        {
            get => selectedMonth;
            set => SetProperty(ref selectedMonth, value);
        }
        private DateTime? startDate = null;
        public DateTime? StartDate
        {
            get => startDate;
            set => SetProperty(ref startDate, value);
        }

        private DateTime? endDate = null;
        public DateTime? EndDate
        {
            get => endDate;
            set => SetProperty(ref endDate, value);
        }

        private int appliedYear;
        public int AppliedYear
        {
            get => appliedYear;
            set => SetProperty(ref appliedYear, value);
        }

        private string appliedMonth;
        public string AppliedMonth
        {
            get => appliedMonth;
            set => SetProperty(ref appliedMonth, value);
        }



        public ICommand LoadAirQualityDataCommand => new Command(async () => await LoadAirQualityDataAsync());
        public ICommand ExportToCsvCommand => new Command(ExportToCsv);
        public ICommand ExportWaterToCsvCommand => new Command(ExportWaterToCsv);
        public ICommand ExportWeatherToCsvCommand => new Command(ExportWeatherToCsv);





        public ICommand ApplyAirQualityFilterCommand => new Command(async () => await ApplyAirQualityFilterAsync());
        /// <summary>
        /// Asynchronously executes the ApplyAirQualityFilterAsync operation.
        /// </summary>
        public async Task ApplyAirQualityFilterAsync()
        {
            AppliedYear = SelectedYear;
            AppliedMonth = SelectedMonth;
            await LoadAirQualityDataAsync(applyFilter: true);
        }

        public HistoricalDataViewerViewModel(HistoricalDataDbContext dbContext, Interfaces.ILoggingService @object)
        {
            Debug.WriteLine("[INFO] Constructor: HistoricalDataViewerViewModel initialized.");
            _dbContext = dbContext;
            SelectedYear = DateTime.Today.Year;
            SelectedMonth = "All";
        }

        /// <summary>
        /// Asynchronously executes the false) operation.
        /// </summary>
        public async Task LoadAirQualityDataAsync(bool applyFilter = false)
        {
            if (IsBusy) return;
            IsBusy = true;
            Debug.WriteLine("[INFO] Loading air quality data...");

            try
            {
                AirQualityData.Clear();
                var query = _dbContext.ArchiveAirQuality.AsQueryable();

                if (applyFilter)
                {
                    query = query.Where(d => d.Date.HasValue && d.Date.Value.Year == AppliedYear);
                    if (AppliedMonth != "All")
                    {
                        int month = AvailableMonths.IndexOf(AppliedMonth);
                        query = query.Where(d => d.Date.Value.Month == month);
                    }
                    if (StartDate.HasValue)
                        query = query.Where(d => d.Date.Value >= StartDate.Value);
                    if (EndDate.HasValue)
                        query = query.Where(d => d.Date.Value <= EndDate.Value);
                }

                var data = await query.OrderByDescending(d => d.Date).ToListAsync();

                foreach (var item in data)
                    AirQualityData.Add(item);

                Debug.WriteLine("[INFO] Air quality data loaded: {0} records", data.Count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] Failed to load air quality data: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine("[INFO] Finished loading air quality data.");
            }
        }


        public ObservableCollection<ArchiveWaterQuality> WaterQualityData { get; } = new();

        /// <summary>
        /// Asynchronously executes the false) operation.
        /// </summary>
        public async Task LoadWaterQualityDataAsync(bool applyFilter = false)
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                WaterQualityData.Clear();
                var query = _dbContext.ArchiveWaterQuality.AsQueryable();

                if (applyFilter)
                {
                    query = query.Where(d => d.Date.HasValue && d.Date.Value.Year == WaterAppliedYear);

                    if (WaterAppliedMonth != "All")
                    {
                        int month = AvailableMonths.IndexOf(WaterAppliedMonth);
                        query = query.Where(d => d.Date.Value.Month == month);
                    }
                }

                var data = await query
                    .OrderByDescending(d => d.Date)
                    .ToListAsync();

                foreach (var item in data)
                    WaterQualityData.Add(item);

                Debug.WriteLine(applyFilter
                    ? $"Loaded {data.Count} filtered water rows for {WaterAppliedMonth} {WaterAppliedYear}"
                    : $"Loaded {data.Count} total water rows");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading water quality data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        /// <summary>
        /// Asynchronously executes the ApplyWaterFilterAsync operation.
        /// </summary>
        public async Task ApplyWaterFilterAsync()
        {
            WaterAppliedYear = WaterSelectedYear;
            WaterAppliedMonth = WaterSelectedMonth;
            await LoadWaterQualityDataAsync(applyFilter: true);
        }

        public ICommand ApplyWaterFilterCommand => new Command(async () => await ApplyWaterFilterAsync());


        private int waterSelectedYear = DateTime.Today.Year;
        public int WaterSelectedYear
        {
            get => waterSelectedYear;
            set => SetProperty(ref waterSelectedYear, value);
        }

        private string waterSelectedMonth = "All";
        public string WaterSelectedMonth
        {
            get => waterSelectedMonth;
            set => SetProperty(ref waterSelectedMonth, value);
        }

        private int waterAppliedYear;
        public int WaterAppliedYear
        {
            get => waterAppliedYear;
            set => SetProperty(ref waterAppliedYear, value);
        }

        private string waterAppliedMonth;
        public string WaterAppliedMonth
        {
            get => waterAppliedMonth;
            set => SetProperty(ref waterAppliedMonth, value);
        }

        public ObservableCollection<ArchiveWeatherData> WeatherData { get; } = new();

        /// <summary>
        /// Asynchronously executes the false) operation.
        /// </summary>
        public async Task LoadWeatherDataAsync(bool applyFilter = false)
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                WeatherData.Clear();
                var query = _dbContext.ArchiveWeatherData.AsQueryable();

                if (applyFilter)
                {
                    query = query.Where(d => d.Date_Time.Year == WeatherAppliedYear);

                    if (WeatherAppliedMonth != "All")
                    {
                        int month = AvailableMonths.IndexOf(WeatherAppliedMonth);
                        query = query.Where(d => d.Date_Time.Month == month);
                    }
                }

                var data = await query
                    .OrderByDescending(d => d.Date_Time)
                    .ToListAsync();

                foreach (var item in data)
                    WeatherData.Add(item);

                Debug.WriteLine(applyFilter
                    ? $"Loaded {data.Count} filtered weather rows for {WeatherAppliedMonth} {WeatherAppliedYear}"
                    : $"Loaded {data.Count} total weather rows");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading weather data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }


        private int weatherSelectedYear = DateTime.Today.Year;
        public int WeatherSelectedYear
        {
            get => weatherSelectedYear;
            set => SetProperty(ref weatherSelectedYear, value);
        }

        private string weatherSelectedMonth = "All";
        public string WeatherSelectedMonth
        {
            get => weatherSelectedMonth;
            set => SetProperty(ref weatherSelectedMonth, value);
        }

        private int weatherAppliedYear;
        public int WeatherAppliedYear
        {
            get => weatherAppliedYear;
            set => SetProperty(ref weatherAppliedYear, value);
        }

        private string weatherAppliedMonth;
        public string WeatherAppliedMonth
        {
            get => weatherAppliedMonth;
            set => SetProperty(ref weatherAppliedMonth, value);
        }

        /// <summary>
        /// Asynchronously executes the ApplyWeatherFilterAsync operation.
        /// </summary>
        public async Task ApplyWeatherFilterAsync()
        {
            WeatherAppliedYear = WeatherSelectedYear;
            WeatherAppliedMonth = WeatherSelectedMonth;
            await LoadWeatherDataAsync(applyFilter: true);
        }

        public ICommand ApplyWeatherFilterCommand => new Command(async () => await ApplyWeatherFilterAsync());

        private void ExportToCsv()
        {
            try
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("Date, Nitrogen_dioxide, Sulphur_dioxide");
                foreach (var data in AirQualityData)
                {
                    csvBuilder.AppendLine($"{data.Date}, {data.Nitrogen_dioxide}, {data.Sulphur_dioxide}");
                }
                string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "AirQualityData.csv");
                File.WriteAllText(filePath, csvBuilder.ToString());
                Debug.WriteLine($"[INFO] Air quality data exported to CSV at: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] Export to CSV failed: " + ex.Message);
            }
        }

        private void ExportWaterToCsv()
        {
            try
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("Date, Nitrate_mg_l_1, Nitrite_less_thank_mg_l_1");
                foreach (var data in WaterQualityData)
                {
                    csvBuilder.AppendLine($"{data.Date}, {data.Nitrate_mg_l_1}, {data.Nitrite_less_thank_mg_l_1}");
                }
                string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "WaterQualityData.csv");
                File.WriteAllText(filePath, csvBuilder.ToString());
                Debug.WriteLine($"[INFO] Water data exported to CSV at: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] Export water to CSV failed: " + ex.Message);
            }
        }

        private void ExportWeatherToCsv()
        {
            try
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("Date_Time, Temperature_2m, Relative_humidity_2m");
                foreach (var data in WeatherData)
                {
                    csvBuilder.AppendLine($"{data.Date_Time}, {data.Temperature_2m}, {data.Relative_humidity_2m}");
                }
                string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "WeatherData.csv");

                File.WriteAllText(filePath, csvBuilder.ToString());
                Debug.WriteLine($"[INFO] Weather data exported to CSV at: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] Export weather to CSV failed: " + ex.Message);
            }
        }
    }

}