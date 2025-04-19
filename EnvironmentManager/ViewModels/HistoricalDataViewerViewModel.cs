using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;


namespace EnvironmentManager.ViewModels
{
    public class HistoricalDataViewerViewModel : BaseViewModel
    {
        private readonly HistoricalDataDbContext _dbContext;

        public ObservableCollection<ArchiveAirQuality> AirQualityData { get; } = new();
        // Dropdown values for year and month filters

        public List<int> AvailableYears { get; } = Enumerable.Range(2020, 10).ToList();

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


        public HistoricalDataViewerViewModel(HistoricalDataDbContext dbContext)
        {
            _dbContext = dbContext;

            // Default values for dropdowns
            SelectedYear = DateTime.Today.Year;
            SelectedMonth = "All";


        }


        public async Task LoadAirQualityDataAsync(bool applyFilter = false)
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                AirQualityData.Clear();

                var query = _dbContext.ArchiveAirQuality.AsQueryable();

                if (applyFilter)
                {
                    query = query.Where(d => d.Date.HasValue && d.Date.Value.Year == AppliedYear);

                    if (AppliedMonth != "All")
                    {
                        int month = AvailableMonths.IndexOf(AppliedMonth); // Jan = 1
                        query = query.Where(d => d.Date.Value.Month == month);
                    }

                    if (StartDate.HasValue)
                        query = query.Where(d => d.Date.Value >= StartDate.Value);

                    if (EndDate.HasValue)
                        query = query.Where(d => d.Date.Value <= EndDate.Value);
                }


                var data = await query
                    .OrderByDescending(d => d.Date)
                    .ToListAsync();

                foreach (var item in data)
                    AirQualityData.Add(item);

                Debug.WriteLine(applyFilter
                    ? $"Loaded {data.Count} filtered rows for {AppliedMonth} {AppliedYear}"
                    : $"Loaded {data.Count} total rows (no filter)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading air quality data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }


        public ICommand ApplyAirQualityFilterCommand => new Command(async () => await ApplyAirQualityFilterAsync());
        public async Task ApplyAirQualityFilterAsync()
        {
            AppliedYear = SelectedYear;
            AppliedMonth = SelectedMonth;
            await LoadAirQualityDataAsync(applyFilter: true);
        }



        public ObservableCollection<ArchiveWaterQuality> WaterQualityData { get; } = new();

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

        public async Task ApplyWeatherFilterAsync()
        {
            WeatherAppliedYear = WeatherSelectedYear;
            WeatherAppliedMonth = WeatherSelectedMonth;
            await LoadWeatherDataAsync(applyFilter: true);
        }

        public ICommand ApplyWeatherFilterCommand => new Command(async () => await ApplyWeatherFilterAsync());

    }

}
