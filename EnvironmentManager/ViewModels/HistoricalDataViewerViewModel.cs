using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace EnvironmentManager.ViewModels
{
    public class HistoricalDataViewerViewModel : BaseViewModel
    {
        private readonly HistoricalDataDbContext _dbContext;

        public ObservableCollection<ArchiveAirQuality> AirQualityData { get; } = new();

        public ICommand LoadAirQualityDataCommand => new Command(async () => await LoadAirQualityDataAsync());

        public HistoricalDataViewerViewModel(HistoricalDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LoadAirQualityDataAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                AirQualityData.Clear();
                var data = await _dbContext.ArchiveAirQuality
                    .OrderByDescending(d => d.Date)
                    .Take(100)
                    .ToListAsync();

                System.Diagnostics.Debug.WriteLine($"Loaded {data.Count} air quality rows");

                foreach (var record in data)
                    AirQualityData.Add(record);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading air quality: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public ObservableCollection<ArchiveWaterQuality> WaterQualityData { get; } = new();

        public async Task LoadWaterQualityDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                WaterQualityData.Clear();
                var data = await _dbContext.ArchiveWaterQuality
                    .OrderByDescending(d => d.Date)
                    .Take(100)
                    .ToListAsync();

                foreach (var item in data)
                    WaterQualityData.Add(item);

                System.Diagnostics.Debug.WriteLine($"Loaded {data.Count} water quality rows");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading water quality data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ObservableCollection<ArchiveWeatherData> WeatherData { get; } = new();

        public async Task LoadWeatherDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                WeatherData.Clear();
                var data = await _dbContext.ArchiveWeatherData
                    .OrderByDescending(d => d.Date_Time)
                    .Take(100)
                    .ToListAsync();

                foreach (var item in data)
                    WeatherData.Add(item);

                System.Diagnostics.Debug.WriteLine($"Loaded {data.Count} weather rows");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading weather data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

    }

}
