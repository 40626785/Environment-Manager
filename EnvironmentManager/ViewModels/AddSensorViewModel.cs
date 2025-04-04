using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using EnvironmentManager.Services;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public partial class AddSensorViewModel : ObservableObject
    {
        private readonly SensorDbContext _context;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LocationErrorVisible))]
        private ObservableCollection<EnvironmentManager.Models.Location> _locations;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LocationErrorVisible))]
        private EnvironmentManager.Models.Location _selectedLocation;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NameErrorVisible))]
        [NotifyPropertyChangedFor(nameof(NameErrorMessage))]
        private string _sensorName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ModelErrorVisible))]
        [NotifyPropertyChangedFor(nameof(ModelErrorMessage))]
        private string _model = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ManufacturerErrorVisible))]
        [NotifyPropertyChangedFor(nameof(ManufacturerErrorMessage))]
        private string _manufacturer = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TypeErrorVisible))]
        [NotifyPropertyChangedFor(nameof(TypeErrorMessage))]
        private string _sensorType = string.Empty;

        [ObservableProperty]
        private DateTime _installationDate = DateTime.Now;

        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FirmwareErrorVisible))]
        [NotifyPropertyChangedFor(nameof(FirmwareErrorMessage))]
        private string _firmwareVersion = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UrlErrorVisible))]
        [NotifyPropertyChangedFor(nameof(UrlErrorMessage))]
        private string _sensorUrl = string.Empty;

        [ObservableProperty]
        private bool _isOnline = false;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BatteryErrorVisible))]
        [NotifyPropertyChangedFor(nameof(BatteryErrorMessage))]
        private float? _batteryLevelPercentage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BatteryErrorVisible))]
        [NotifyPropertyChangedFor(nameof(BatteryErrorMessage))]
        private string _batteryLevelText = string.Empty;

        private Dictionary<string, string> _validationErrors = new();

        public bool LocationErrorVisible => SelectedLocation == null && _validationErrors.ContainsKey("Location");
        public bool NameErrorVisible => _validationErrors.ContainsKey("SensorName");
        public bool ModelErrorVisible => _validationErrors.ContainsKey("Model");
        public bool ManufacturerErrorVisible => _validationErrors.ContainsKey("Manufacturer");
        public bool TypeErrorVisible => _validationErrors.ContainsKey("SensorType");
        public bool FirmwareErrorVisible => _validationErrors.ContainsKey("FirmwareVersion");
        public bool UrlErrorVisible => _validationErrors.ContainsKey("SensorUrl");
        public bool BatteryErrorVisible => _validationErrors.ContainsKey("BatteryLevel");

        public string NameErrorMessage => _validationErrors.GetValueOrDefault("SensorName", string.Empty);
        public string ModelErrorMessage => _validationErrors.GetValueOrDefault("Model", string.Empty);
        public string ManufacturerErrorMessage => _validationErrors.GetValueOrDefault("Manufacturer", string.Empty);
        public string TypeErrorMessage => _validationErrors.GetValueOrDefault("SensorType", string.Empty);
        public string FirmwareErrorMessage => _validationErrors.GetValueOrDefault("FirmwareVersion", string.Empty);
        public string UrlErrorMessage => _validationErrors.GetValueOrDefault("SensorUrl", string.Empty);
        public string BatteryErrorMessage => _validationErrors.GetValueOrDefault("BatteryLevel", string.Empty);

        public AddSensorViewModel(SensorDbContext context)
        {
            _context = context;
            _locations = new ObservableCollection<EnvironmentManager.Models.Location>();
            LoadLocations();
            
            // Set up handlers for property changes
            PropertyChanged += (sender, args) => 
            {
                switch (args.PropertyName)
                {
                    case nameof(BatteryLevelText):
                        ValidateBatteryLevel();
                        break;
                    case nameof(SensorName):
                        ValidateSensorName();
                        break;
                    case nameof(Model):
                        ValidateModel();
                        break;
                    case nameof(Manufacturer):
                        ValidateManufacturer();
                        break;
                    case nameof(SensorType):
                        ValidateSensorType();
                        break;
                    case nameof(FirmwareVersion):
                        ValidateFirmwareVersion();
                        break;
                    case nameof(SensorUrl):
                        ValidateSensorUrl();
                        break;
                }
            };
        }
        
        private void ValidateBatteryLevel()
        {
            _validationErrors.Remove("BatteryLevel");

            if (string.IsNullOrWhiteSpace(BatteryLevelText))
            {
                BatteryLevelPercentage = null;
                OnPropertyChanged(nameof(BatteryErrorVisible));
                OnPropertyChanged(nameof(BatteryErrorMessage));
                return;
            }

            if (!float.TryParse(BatteryLevelText, out float value))
            {
                _validationErrors["BatteryLevel"] = "Battery level must be a valid number";
                BatteryLevelPercentage = null;
            }
            else if (value < 0 || value > 100)
            {
                _validationErrors["BatteryLevel"] = "Battery level must be between 0 and 100";
                BatteryLevelPercentage = null;
            }
            else
            {
                BatteryLevelPercentage = value;
            }

            OnPropertyChanged(nameof(BatteryErrorVisible));
            OnPropertyChanged(nameof(BatteryErrorMessage));
        }

        private void ValidateSensorName()
        {
            _validationErrors.Remove("SensorName");
            if (string.IsNullOrWhiteSpace(SensorName))
            {
                _validationErrors["SensorName"] = "Sensor name is required";
            }
            else if (SensorName.Length > 100)
            {
                _validationErrors["SensorName"] = "Sensor name must be 100 characters or less";
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(SensorName, @"^\d+$"))
            {
                _validationErrors["SensorName"] = "Sensor name cannot contain only numbers";
            }
            OnPropertyChanged(nameof(NameErrorVisible));
            OnPropertyChanged(nameof(NameErrorMessage));
        }

        private void ValidateModel()
        {
            _validationErrors.Remove("Model");
            if (string.IsNullOrWhiteSpace(Model))
            {
                _validationErrors["Model"] = "Model is required";
            }
            else if (Model.Length > 100)
            {
                _validationErrors["Model"] = "Model must be 100 characters or less";
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(Model, @"^\d+$"))
            {
                _validationErrors["Model"] = "Model cannot contain only numbers";
            }
            OnPropertyChanged(nameof(ModelErrorVisible));
            OnPropertyChanged(nameof(ModelErrorMessage));
        }

        private void ValidateManufacturer()
        {
            _validationErrors.Remove("Manufacturer");
            if (string.IsNullOrWhiteSpace(Manufacturer))
            {
                _validationErrors["Manufacturer"] = "Manufacturer is required";
            }
            else if (Manufacturer.Length > 100)
            {
                _validationErrors["Manufacturer"] = "Manufacturer must be 100 characters or less";
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(Manufacturer, @"^\d+$"))
            {
                _validationErrors["Manufacturer"] = "Manufacturer cannot contain only numbers";
            }
            OnPropertyChanged(nameof(ManufacturerErrorVisible));
            OnPropertyChanged(nameof(ManufacturerErrorMessage));
        }

        private void ValidateSensorType()
        {
            _validationErrors.Remove("SensorType");
            if (string.IsNullOrWhiteSpace(SensorType))
            {
                _validationErrors["SensorType"] = "Sensor type is required";
            }
            else if (SensorType.Length > 50)
            {
                _validationErrors["SensorType"] = "Sensor type must be 50 characters or less";
            }
            OnPropertyChanged(nameof(TypeErrorVisible));
            OnPropertyChanged(nameof(TypeErrorMessage));
        }

        private void ValidateFirmwareVersion()
        {
            _validationErrors.Remove("FirmwareVersion");
            if (string.IsNullOrWhiteSpace(FirmwareVersion))
            {
                _validationErrors["FirmwareVersion"] = "Firmware version is required";
            }
            else
            {
                var (isValid, errorMessage) = ValidationService.ValidateVersion(FirmwareVersion);
                if (!isValid)
                {
                    _validationErrors["FirmwareVersion"] = errorMessage;
                }
            }
            OnPropertyChanged(nameof(FirmwareErrorVisible));
            OnPropertyChanged(nameof(FirmwareErrorMessage));
        }

        private void ValidateSensorUrl()
        {
            _validationErrors.Remove("SensorUrl");
            if (string.IsNullOrWhiteSpace(SensorUrl))
            {
                _validationErrors["SensorUrl"] = "Sensor URL is required";
            }
            else
            {
                var (isValid, errorMessage) = ValidationService.ValidateUrl(SensorUrl);
                if (!isValid)
                {
                    _validationErrors["SensorUrl"] = errorMessage;
                }
            }
            OnPropertyChanged(nameof(UrlErrorVisible));
            OnPropertyChanged(nameof(UrlErrorMessage));
        }

        private async void LoadLocations()
        {
            try
            {
                var locationsList = await _context.Locations
                    .AsNoTracking()
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

        private bool ValidateForm()
        {
            _validationErrors.Clear();

            if (SelectedLocation == null)
            {
                _validationErrors["Location"] = "Please select a location";
            }

            var (isValid, errors) = ValidationService.ValidateSensor(
                SensorName,
                Model,
                Manufacturer,
                SensorType,
                FirmwareVersion,
                SensorUrl,
                BatteryLevelPercentage
            );

            if (!isValid)
            {
                foreach (var error in errors)
                {
                    _validationErrors[error.Key] = error.Value;
                }
            }

            // Validate battery level again to ensure it's valid
            if (!string.IsNullOrWhiteSpace(BatteryLevelText))
            {
                ValidateBatteryLevel();
            }

            // Notify UI of validation changes
            OnPropertyChanged(nameof(LocationErrorVisible));
            OnPropertyChanged(nameof(NameErrorVisible));
            OnPropertyChanged(nameof(ModelErrorVisible));
            OnPropertyChanged(nameof(ManufacturerErrorVisible));
            OnPropertyChanged(nameof(TypeErrorVisible));
            OnPropertyChanged(nameof(FirmwareErrorVisible));
            OnPropertyChanged(nameof(UrlErrorVisible));
            OnPropertyChanged(nameof(BatteryErrorVisible));

            OnPropertyChanged(nameof(NameErrorMessage));
            OnPropertyChanged(nameof(ModelErrorMessage));
            OnPropertyChanged(nameof(ManufacturerErrorMessage));
            OnPropertyChanged(nameof(TypeErrorMessage));
            OnPropertyChanged(nameof(FirmwareErrorMessage));
            OnPropertyChanged(nameof(UrlErrorMessage));
            OnPropertyChanged(nameof(BatteryErrorMessage));

            return !_validationErrors.Any();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (!ValidateForm())
            {
                await Shell.Current.DisplayAlert("Validation Error", 
                    "Please correct the errors before saving.", "OK");
                return;
            }

            try
            {
                var sensor = new Sensor
                {
                    LocationId = SelectedLocation?.LocationId ?? 0,
                    SensorName = SensorName,
                    Model = Model,
                    Manufacturer = Manufacturer,
                    SensorType = SensorType,
                    InstallationDate = InstallationDate,
                    IsActive = IsActive,
                    FirmwareVersion = FirmwareVersion,
                    SensorUrl = SensorUrl,
                    ConnectivityStatus = IsOnline ? "Online" : "Offline",
                    BatteryLevelPercentage = BatteryLevelPercentage
                };

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