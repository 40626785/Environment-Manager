using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public partial class AddSensorViewModel : ObservableObject
    {
        private readonly SensorDbContext _context;

        [ObservableProperty]
        private ObservableCollection<EnvironmentManager.Models.Location> _locations;

        [ObservableProperty]
        private EnvironmentManager.Models.Location _selectedLocation;

        [ObservableProperty]
        private string _sensorName = string.Empty;

        [ObservableProperty]
        private string _model = string.Empty;

        [ObservableProperty]
        private string _manufacturer = string.Empty;

        [ObservableProperty]
        private string _sensorType = string.Empty;

        [ObservableProperty]
        private DateTime _installationDate = DateTime.Now;

        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        private string _dataSource = string.Empty;

        [ObservableProperty]
        private string _firmwareVersion = string.Empty;

        [ObservableProperty]
        private string _sensorUrl = string.Empty;

        [ObservableProperty]
        private bool _isOnline = false;

        [ObservableProperty]
        private float? _batteryLevelPercentage;

        public AddSensorViewModel(SensorDbContext context)
        {
            _context = context;
            _locations = new ObservableCollection<EnvironmentManager.Models.Location>();
            LoadLocations();
        }

        private async void LoadLocations()
        {
            try
            {
                var locationsList = await _context.Locations
                    .OrderBy(l => l.SiteName)
                    .ToListAsync();

                Locations.Clear();
                foreach (var location in locationsList)
                {
                    Locations.Add(location);
                }

                // Select first location by default if available
                if (Locations.Any())
                {
                    SelectedLocation = Locations.First();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load locations: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(SensorName))
            {
                await Shell.Current.DisplayAlert("Error", "Sensor Name is required.", "OK");
                return;
            }

            if (SelectedLocation == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a location.", "OK");
                return;
            }

            var sensor = new Sensor
            {
                LocationId = SelectedLocation.LocationId,
                SensorName = SensorName,
                Model = Model,
                Manufacturer = Manufacturer,
                SensorType = SensorType,
                InstallationDate = InstallationDate,
                IsActive = IsActive,
                DataSource = DataSource,
                FirmwareVersion = FirmwareVersion,
                SensorUrl = SensorUrl,
                ConnectivityStatus = IsOnline ? "Online" : "Offline",
                BatteryLevelPercentage = BatteryLevelPercentage
            };

            try
            {
                _context.Sensors.Add(sensor);
                await _context.SaveChangesAsync();
                await Shell.Current.DisplayAlert("Success", "Sensor added successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to add sensor: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
} 