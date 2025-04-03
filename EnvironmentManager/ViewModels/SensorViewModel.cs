using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EnvironmentManager.ViewModels
{
    public partial class SensorViewModel : ObservableObject
    {
        private readonly MaintenanceDbContext _context;

        [ObservableProperty]
        private ObservableCollection<Sensor> _sensors;

        [ObservableProperty]
        private Sensor? _selectedSensor;

        [ObservableProperty]
        private string _sensorName;
        [ObservableProperty]
        private string _model;
        [ObservableProperty]
        private string _manufacturer;
        [ObservableProperty]
        private string _sensorType;
        [ObservableProperty]
        private DateTime _installationDate = DateTime.Now;
        [ObservableProperty]
        private bool _isActive = true;
        [ObservableProperty]
        private DateTime? _lastCalibration;
        [ObservableProperty]
        private string _firmwareVersion;
        [ObservableProperty]
        private string _dataSource;
        [ObservableProperty]
        private string _sensorUrl;
        [ObservableProperty]
        private DateTime? _lastHeartbeat;
        [ObservableProperty]
        private string _connectivityStatus;
        [ObservableProperty]
        private float? _batteryLevelPercentage;
        [ObservableProperty]
        private int? _signalStrengthDbm;

        [ObservableProperty]
        private bool _isEditing = false;

        [ObservableProperty]
        private string _pageTitle = "Add New Sensor";

        public SensorViewModel(MaintenanceDbContext context)
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
            Sensors.Clear();
            var sensorsList = await _context.Sensors.OrderBy(s => s.SensorName).ToListAsync();
            foreach (var sensor in sensorsList)
            {
                Sensors.Add(sensor);
            }
        }

        [RelayCommand]
        private void PrepareNewSensor()
        {
            SelectedSensor = null;
            IsEditing = false;

            SensorName = string.Empty;
            Model = string.Empty;
            Manufacturer = string.Empty;
            SensorType = string.Empty;
            InstallationDate = DateTime.Now;
            IsActive = true;
            LastCalibration = null;
            FirmwareVersion = string.Empty;
            DataSource = string.Empty;
            SensorUrl = string.Empty;
            LastHeartbeat = null;
            ConnectivityStatus = string.Empty;
            BatteryLevelPercentage = null;
            SignalStrengthDbm = null;
        }

        [RelayCommand]
        private void SelectSensorForEdit(Sensor? sensor)
        {
            if (sensor == null) return;

            SelectedSensor = sensor;
            IsEditing = true;

            SensorName = sensor.SensorName;
            Model = sensor.Model;
            Manufacturer = sensor.Manufacturer;
            SensorType = sensor.SensorType;
            InstallationDate = sensor.InstallationDate;
            IsActive = sensor.IsActive;
            LastCalibration = sensor.LastCalibration;
            FirmwareVersion = sensor.FirmwareVersion;
            DataSource = sensor.DataSource;
            SensorUrl = sensor.SensorUrl;
            LastHeartbeat = sensor.LastHeartbeat;
            ConnectivityStatus = sensor.ConnectivityStatus;
            BatteryLevelPercentage = sensor.BatteryLevelPercentage;
            SignalStrengthDbm = sensor.SignalStrengthDbm;
        }

        [RelayCommand]
        private async Task SaveSensorAsync()
        {
            if (string.IsNullOrWhiteSpace(SensorName))
            {
                await Shell.Current.DisplayAlert("Error", "Sensor Name is required.", "OK");
                return;
            }

            Sensor sensorToSave;

            if (IsEditing && SelectedSensor != null)
            {
                sensorToSave = SelectedSensor;
            }
            else
            {
                sensorToSave = new Sensor();
                _context.Sensors.Add(sensorToSave);
            }

            sensorToSave.SensorName = SensorName;
            sensorToSave.Model = Model;
            sensorToSave.Manufacturer = Manufacturer;
            sensorToSave.SensorType = SensorType;
            sensorToSave.InstallationDate = InstallationDate;
            sensorToSave.IsActive = IsActive;
            sensorToSave.LastCalibration = LastCalibration;
            sensorToSave.FirmwareVersion = FirmwareVersion;
            sensorToSave.DataSource = DataSource;
            sensorToSave.SensorUrl = SensorUrl;
            sensorToSave.LastHeartbeat = LastHeartbeat;
            sensorToSave.ConnectivityStatus = ConnectivityStatus;
            sensorToSave.BatteryLevelPercentage = BatteryLevelPercentage;
            sensorToSave.SignalStrengthDbm = SignalStrengthDbm;

            try
            {
                await _context.SaveChangesAsync();
                PrepareNewSensor();
                await LoadSensorsAsync();
                await Shell.Current.DisplayAlert("Success", "Sensor saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save sensor: {ex.Message}", "OK");
                if (!IsEditing)
                {
                    _context.Entry(sensorToSave).State = EntityState.Detached;
                }
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
                _context.Sensors.Remove(sensor);
                await _context.SaveChangesAsync();

                if (SelectedSensor == sensor)
                {
                    PrepareNewSensor();
                }
                await LoadSensorsAsync();
                await Shell.Current.DisplayAlert("Success", "Sensor deleted successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to delete sensor: {ex.Message}", "OK");
                _context.Entry(sensor).State = EntityState.Unchanged;
            }
        }
    }
}
