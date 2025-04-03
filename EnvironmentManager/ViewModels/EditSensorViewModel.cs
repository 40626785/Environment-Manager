using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    [QueryProperty(nameof(SensorId), "id")]
    public partial class EditSensorViewModel : ObservableObject
    {
        private readonly SensorDbContext _context;

        [ObservableProperty]
        private int _sensorId;

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

        public EditSensorViewModel(SensorDbContext context)
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
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load locations: {ex.Message}", "OK");
            }
        }

        partial void OnSensorIdChanged(int value)
        {
            LoadSensor(value);
        }

        private async void LoadSensor(int sensorId)
        {
            var sensor = await _context.Sensors
                .Include(s => s.Location)
                .FirstOrDefaultAsync(s => s.SensorId == sensorId);

            if (sensor != null)
            {
                SensorName = sensor.SensorName;
                Model = sensor.Model;
                Manufacturer = sensor.Manufacturer;
                SensorType = sensor.SensorType;
                InstallationDate = sensor.InstallationDate;
                IsActive = sensor.IsActive;
                DataSource = sensor.DataSource;
                FirmwareVersion = sensor.FirmwareVersion;
                SensorUrl = sensor.SensorUrl;
                IsOnline = sensor.ConnectivityStatus == "Online";
                BatteryLevelPercentage = sensor.BatteryLevelPercentage;

                // Set selected location
                SelectedLocation = Locations.FirstOrDefault(l => l.LocationId == sensor.LocationId);
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

            var sensor = await _context.Sensors.FindAsync(SensorId);
            if (sensor == null)
            {
                await Shell.Current.DisplayAlert("Error", "Sensor not found.", "OK");
                return;
            }

            sensor.LocationId = SelectedLocation.LocationId;
            sensor.SensorName = SensorName;
            sensor.Model = Model;
            sensor.Manufacturer = Manufacturer;
            sensor.SensorType = SensorType;
            sensor.InstallationDate = InstallationDate;
            sensor.IsActive = IsActive;
            sensor.DataSource = DataSource;
            sensor.FirmwareVersion = FirmwareVersion;
            sensor.SensorUrl = SensorUrl;
            sensor.ConnectivityStatus = IsOnline ? "Online" : "Offline";
            sensor.BatteryLevelPercentage = BatteryLevelPercentage;

            try
            {
                await _context.SaveChangesAsync();
                await Shell.Current.DisplayAlert("Success", "Sensor updated successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to update sensor: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            var sensor = await _context.Sensors.FindAsync(SensorId);
            if (sensor == null)
            {
                await Shell.Current.DisplayAlert("Error", "Sensor not found.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirm Delete", 
                $"Are you sure you want to delete sensor '{sensor.SensorName}'?", 
                "Yes", 
                "No");

            if (!confirm) return;

            try
            {
                _context.Sensors.Remove(sensor);
                await _context.SaveChangesAsync();
                await Shell.Current.DisplayAlert("Success", "Sensor deleted successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to delete sensor: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
} 