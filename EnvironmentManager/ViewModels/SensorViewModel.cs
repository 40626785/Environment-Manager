using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics;

namespace EnvironmentManager.ViewModels
{
    public partial class SensorViewModel : ObservableObject
    {
        private readonly SensorDbContext _context;
        private bool _isLoading;

        [ObservableProperty]
        private ObservableCollection<Sensor> _sensors;

        [ObservableProperty]
        private Sensor? _selectedSensor;

        [ObservableProperty]
        private int _sensorId;

        [ObservableProperty]
        private int _locationId;

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
        private bool _isEditing = false;

        [ObservableProperty]
        private string _pageTitle = "Add New Sensor";

        public SensorViewModel(SensorDbContext context)
        {
            _context = context;
            _sensors = new ObservableCollection<Sensor>();
            UpdatePageTitle();
            LoadSensorsCommand.ExecuteAsync(null);
        }

        partial void OnIsEditingChanged(bool value)
        {
            UpdatePageTitle();
        }

        private void UpdatePageTitle()
        {
            PageTitle = IsEditing ? "Edit Sensor" : "Add New Sensor";
        }

        [RelayCommand]
        private async Task LoadSensorsAsync()
        {
            if (_isLoading)
            {
                Debug.WriteLine("Load operation already in progress, skipping...");
                return;
            }

            try
            {
                _isLoading = true;
                Debug.WriteLine("Starting to load sensors...");
                Sensors.Clear();
                
                // Check if context is available
                if (_context == null)
                {
                    Debug.WriteLine("Error: Database context is null");
                    await Shell.Current.DisplayAlert("Error", "Database context is not initialized", "OK");
                    return;
                }

                Debug.WriteLine("Attempting to query sensors from database...");
                var sensorsList = await _context.Sensors
                    .Include(s => s.Location)  // Include location data
                    .AsNoTracking()
                    .OrderBy(s => s.SensorName)
                    .ToListAsync();
                
                Debug.WriteLine($"Found {sensorsList.Count} sensors");
                foreach (var sensor in sensorsList)
                {
                    Debug.WriteLine($"Sensor Details:");
                    Debug.WriteLine($"  ID: {sensor.SensorId}");
                    Debug.WriteLine($"  Name: {sensor.SensorName}");
                    Debug.WriteLine($"  Type: {sensor.SensorType}");
                    Debug.WriteLine($"  Location ID: {sensor.LocationId}");
                    Debug.WriteLine($"  Location Name: {sensor.Location?.SiteName}");
                    Debug.WriteLine($"  Location Type: {sensor.Location?.SiteType}");
                    Debug.WriteLine($"  Installation Date: {sensor.InstallationDate}");
                    Debug.WriteLine($"  Status: {sensor.ConnectivityStatus}");
                    Sensors.Add(sensor);
                }
                Debug.WriteLine("Successfully loaded sensors");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading sensors: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Error", $"Failed to load sensors: {ex.Message}", "OK");
            }
            finally
            {
                _isLoading = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToAddAsync()
        {
            await Shell.Current.GoToAsync("AddSensor");
        }

        [RelayCommand]
        private async Task NavigateToEditAsync(Sensor sensor)
        {
            if (sensor == null) return;
            
            var parameters = new Dictionary<string, object>
            {
                { "id", sensor.SensorId }
            };
            
            await Shell.Current.GoToAsync("EditSensor", parameters);
        }

        [RelayCommand]
        private void PrepareNewSensor()
        {
            SelectedSensor = null;
            IsEditing = false;

            SensorId = 0;
            LocationId = 0;
            SensorName = string.Empty;
            Model = string.Empty;
            Manufacturer = string.Empty;
            SensorType = string.Empty;
            InstallationDate = DateTime.Now;
            IsActive = true;
            DataSource = string.Empty;
        }

        [RelayCommand]
        private void SelectSensorForEdit(Sensor? sensor)
        {
            if (sensor == null) return;

            SelectedSensor = sensor;
            IsEditing = true;

            SensorId = sensor.SensorId;
            LocationId = sensor.LocationId;
            SensorName = sensor.SensorName;
            Model = sensor.Model;
            Manufacturer = sensor.Manufacturer;
            SensorType = sensor.SensorType;
            InstallationDate = sensor.InstallationDate;
            IsActive = sensor.IsActive;
            DataSource = sensor.DataSource;
        }

        [RelayCommand]
        private async Task SaveSensorAsync()
        {
            if (string.IsNullOrWhiteSpace(SensorName))
            {
                await Shell.Current.DisplayAlert("Error", "Sensor Name is required.", "OK");
                return;
            }

            try
            {
                Sensor sensorToSave;

                if (IsEditing && SelectedSensor != null)
                {
                    sensorToSave = await _context.Sensors.FindAsync(SelectedSensor.SensorId);
                    if (sensorToSave == null)
                    {
                        await Shell.Current.DisplayAlert("Error", "Sensor not found.", "OK");
                        return;
                    }
                }
                else
                {
                    sensorToSave = new Sensor();
                    _context.Sensors.Add(sensorToSave);
                }

                sensorToSave.LocationId = LocationId;
                sensorToSave.SensorName = SensorName;
                sensorToSave.Model = Model;
                sensorToSave.Manufacturer = Manufacturer;
                sensorToSave.SensorType = SensorType;
                sensorToSave.InstallationDate = InstallationDate;
                sensorToSave.IsActive = IsActive;
                sensorToSave.DataSource = DataSource;

                await _context.SaveChangesAsync();
                PrepareNewSensor();
                await LoadSensorsAsync();
                await Shell.Current.DisplayAlert("Success", "Sensor saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save sensor: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteSensorAsync(Sensor? sensor)
        {
            if (sensor == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirm Delete", $"Are you sure you want to delete sensor '{sensor.SensorName}'?", "Yes", "No");
            if (!confirm) return;

            try
            {
                var sensorToDelete = await _context.Sensors.FindAsync(sensor.SensorId);
                if (sensorToDelete == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Sensor not found.", "OK");
                    return;
                }

                _context.Sensors.Remove(sensorToDelete);
                await _context.SaveChangesAsync();

                if (SelectedSensor?.SensorId == sensor.SensorId)
                {
                    PrepareNewSensor();
                }
                await LoadSensorsAsync();
                await Shell.Current.DisplayAlert("Success", "Sensor deleted successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to delete sensor: {ex.Message}", "OK");
            }
        }
    }
}
