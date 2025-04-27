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
    /// <summary>
    /// ViewModel for managing sensor data, including displaying, adding, editing, and deleting sensors.
    /// Interacts with the SensorDbContext to persist changes.
    /// </summary>
    public partial class SensorViewModel : ObservableObject
    {
        private readonly SensorDbContext _context;
        private bool _isLoading; // Tracks if data is currently being loaded

        /// <summary>
        /// Collection of sensors to be displayed.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Sensor> _sensors;

        /// <summary>
        /// The currently selected sensor in the UI.
        /// </summary>
        [ObservableProperty]
        private Sensor? _selectedSensor;

        // Properties bound to the Add/Edit sensor form fields
        /// <summary>
        /// Gets or sets the ID of the sensor being added or edited.
        /// </summary>
        [ObservableProperty]
        private int _sensorId;

        /// <summary>
        /// Gets or sets the Location ID associated with the sensor.
        /// </summary>
        [ObservableProperty]
        private int _locationId;

        /// <summary>
        /// Gets or sets the name of the sensor.
        /// </summary>
        [ObservableProperty]
        private string _sensorName = string.Empty;

        /// <summary>
        /// Gets or sets the model name/number of the sensor.
        /// </summary>
        [ObservableProperty]
        private string _model = string.Empty;

        /// <summary>
        /// Gets or sets the manufacturer of the sensor.
        /// </summary>
        [ObservableProperty]
        private string _manufacturer = string.Empty;

        /// <summary>
        /// Gets or sets the type of the sensor (e.g., Air Quality, Water Quality).
        /// </summary>
        [ObservableProperty]
        private string _sensorType = string.Empty;

        /// <summary>
        /// Gets or sets the installation date of the sensor.
        /// </summary>
        [ObservableProperty]
        private DateTime _installationDate = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether the sensor is currently active.
        /// </summary>
        [ObservableProperty]
        private bool _isActive = true;

        /// <summary>
        /// Gets or sets the data source for the sensor's readings.
        /// </summary>
        [ObservableProperty]
        private string _dataSource = string.Empty;

        /// <summary>
        /// Gets or sets the firmware version of the sensor.
        /// </summary>
        [ObservableProperty]
        private string _firmwareVersion = string.Empty;

        /// <summary>
        /// Gets or sets the URL associated with the sensor (e.g., manufacturer's page).
        /// </summary>
        [ObservableProperty]
        private string _sensorUrl = string.Empty;

        /// <summary>
        /// Gets or sets the connectivity status (e.g., Online, Offline).
        /// </summary>
        [ObservableProperty]
        private string _connectivityStatus = string.Empty;

        /// <summary>
        /// Gets or sets the battery level percentage (nullable).
        /// </summary>
        [ObservableProperty]
        private float? _batteryLevelPercentage;

        /// <summary>
        /// Gets or sets the battery level as a displayable string.
        /// </summary>
        [ObservableProperty]
        private string _batteryLevelText = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is in editing mode (vs. adding mode).
        /// </summary>
        [ObservableProperty]
        private bool _isEditing = false;

        /// <summary>
        /// Gets or sets the title displayed on the associated page (e.g., "Add Sensor" or "Edit Sensor").
        /// </summary>
        [ObservableProperty]
        private string _pageTitle = "Add New Sensor";

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorViewModel"/> class.
        /// </summary>
        /// <param name="context">The database context for sensor data.</param>
        public SensorViewModel(SensorDbContext context)
        {
            _context = context;
            _sensors = new ObservableCollection<Sensor>();
            UpdatePageTitle();
            // Load sensors initially when the ViewModel is created
            LoadSensorsCommand.ExecuteAsync(null);
        }

        // Updates the page title when the editing state changes
        partial void OnIsEditingChanged(bool value)
        {
            UpdatePageTitle();
        }

        // Helper method to set the page title based on editing state
        private void UpdatePageTitle()
        {
            PageTitle = IsEditing ? "Edit Sensor" : "Add New Sensor";
        }

        /// <summary>
        /// Asynchronously loads the list of sensors from the database.
        /// Includes related Location data.
        /// </summary>
        [RelayCommand]
        private async Task LoadSensorsAsync()
        {
            if (_isLoading) return; // Prevent concurrent loading
            try
            {
                _isLoading = true;
                Debug.WriteLine("Starting to load sensors...");
                Sensors.Clear();

                if (_context == null)
                {
                    Debug.WriteLine("Error: Database context is not initialized in SensorViewModel.");
                    // Optionally display an alert, though relying on Debug for VM issues might be better
                    // if (Shell.Current != null) { await Shell.Current.DisplayAlert("Error", "Database context is not initialized", "OK"); }
                    return;
                }

                var sensorsList = await _context.Sensors
                    .Include(s => s.Location) // Eager load location data
                    .AsNoTracking()           // Improve performance for read-only list
                    .OrderBy(s => s.SensorName)
                    .ToListAsync();

                foreach (var sensor in sensorsList)
                {
                    Sensors.Add(sensor);
                }
                Debug.WriteLine($"Loaded {Sensors.Count} sensors.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading sensors: {ex.Message}");
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to load sensors: {ex.Message}", "OK");
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        /// <summary>
        /// Navigates to the page for adding a new sensor.
        /// </summary>
        [RelayCommand]
        private async Task NavigateToAddAsync()
        {
            // Consider clearing form fields here if navigating from the main list view
            // PrepareNewSensor(); // Uncomment if AddSensorPage reuses this ViewModel instance directly
            await Shell.Current.GoToAsync(nameof(Views.AddSensorPage));
        }

        /// <summary>
        /// Navigates to the page for editing the specified sensor.
        /// </summary>
        /// <param name="sensor">The sensor to edit.</param>
        [RelayCommand]
        private async Task NavigateToEditAsync(Sensor sensor)
        {
            if (sensor == null)
            {
                Debug.WriteLine("NavigateToEditAsync called with null sensor.");
                return;
            }

            // Pass the ID of the sensor to the edit page
            var parameters = new Dictionary<string, object>
            {
                { "id", sensor.SensorId }
            };
            Debug.WriteLine($"Navigating to edit sensor with ID: {sensor.SensorId}");
            await Shell.Current.GoToAsync(nameof(Views.EditSensorPage), parameters);
        }

        /// <summary>
        /// Resets the form fields to prepare for adding a new sensor.
        /// </summary>
        [RelayCommand]
        private void PrepareNewSensor()
        {
            SelectedSensor = null;
            IsEditing = false;

            // Reset all properties bound to the form
            SensorId = 0;
            LocationId = 0; // Consider setting a default or prompting
            SensorName = string.Empty;
            Model = string.Empty;
            Manufacturer = string.Empty;
            SensorType = string.Empty; // Consider providing default options
            InstallationDate = DateTime.Now;
            IsActive = true;
            DataSource = string.Empty;
            FirmwareVersion = string.Empty;
            SensorUrl = string.Empty;
            ConnectivityStatus = string.Empty; // May need a default like "Unknown"
            BatteryLevelPercentage = null;
            BatteryLevelText = string.Empty;
            Debug.WriteLine("Prepared form for new sensor.");
        }

        /// <summary>
        /// Populates the form fields with the data of the selected sensor for editing.
        /// </summary>
        /// <param name="sensor">The sensor selected for editing.</param>
        [RelayCommand]
        private void SelectSensorForEdit(Sensor? sensor)
        {
            if (sensor == null)
            {
                 Debug.WriteLine("SelectSensorForEdit called with null sensor.");
                 PrepareNewSensor(); // Reset form if selection is cleared
                 return;
            }

            SelectedSensor = sensor;
            IsEditing = true;

            // Populate form fields from the selected sensor
            SensorId = sensor.SensorId;
            LocationId = sensor.LocationId;
            SensorName = sensor.SensorName;
            Model = sensor.Model;
            Manufacturer = sensor.Manufacturer;
            SensorType = sensor.SensorType;
            InstallationDate = sensor.InstallationDate;
            IsActive = sensor.IsActive;
            DataSource = sensor.DataSource ?? string.Empty; // Handle potential nulls
            FirmwareVersion = sensor.FirmwareVersion ?? string.Empty;
            SensorUrl = sensor.SensorUrl ?? string.Empty;
            ConnectivityStatus = sensor.ConnectivityStatus ?? string.Empty;
            BatteryLevelPercentage = sensor.BatteryLevelPercentage;
            BatteryLevelText = sensor.BatteryLevelPercentage?.ToString() ?? string.Empty;
            Debug.WriteLine($"Selected sensor ID {SensorId} for editing.");
        }

        /// <summary>
        /// Saves the current sensor data (either new or edited) to the database.
        /// </summary>
        [RelayCommand]
        private async Task SaveSensorAsync()
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(SensorName))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Sensor Name is required.", "OK");
                return;
            }
            // Add more validation as needed (e.g., LocationId selected)

            try
            {
                Sensor sensorToSave;

                if (IsEditing && SelectedSensor != null)
                {
                    // Find the existing sensor to update
                    Debug.WriteLine($"Attempting to find sensor with ID: {SelectedSensor.SensorId} for update.");
                    sensorToSave = await _context.Sensors.FindAsync(SelectedSensor.SensorId);
                    if (sensorToSave == null)
                    {
                        Debug.WriteLine($"Error: Sensor with ID {SelectedSensor.SensorId} not found for update.");
                        await Shell.Current.DisplayAlert("Error", "Sensor not found.", "OK");
                        return;
                    }
                     Debug.WriteLine($"Updating sensor ID: {sensorToSave.SensorId}");
                }
                else
                {
                     Debug.WriteLine("Adding new sensor.");
                    // Create a new sensor instance
                    sensorToSave = new Sensor();
                    _context.Sensors.Add(sensorToSave);
                }

                // Map ViewModel properties to the entity
                sensorToSave.LocationId = LocationId;
                sensorToSave.SensorName = SensorName;
                sensorToSave.Model = Model;
                sensorToSave.Manufacturer = Manufacturer;
                sensorToSave.SensorType = SensorType;
                sensorToSave.InstallationDate = InstallationDate;
                sensorToSave.IsActive = IsActive;
                sensorToSave.DataSource = DataSource;
                sensorToSave.FirmwareVersion = FirmwareVersion;
                sensorToSave.SensorUrl = SensorUrl;
                sensorToSave.ConnectivityStatus = ConnectivityStatus;
                sensorToSave.BatteryLevelPercentage = BatteryLevelPercentage;
                // Ensure Location navigation property is handled if needed, EF might handle it via LocationId

                await _context.SaveChangesAsync();
                Debug.WriteLine($"Sensor ID {sensorToSave.SensorId} saved successfully.");

                // Reset form state and refresh list
                PrepareNewSensor();
                // Use MessagingCenter or WeakReferenceMessenger for updates if list is on a different page/VM
                // MessagingCenter.Send(this, "SensorUpdated", sensorToSave); // Example using obsolete MessagingCenter
                await LoadSensorsAsync(); // Reload the list on the current page
                await Shell.Current.DisplayAlert("Success", "Sensor saved successfully.", "OK");
                // Consider navigating back automatically after save
                // if (Shell.Current.Navigation.NavigationStack.Count > 1)
                // {
                //     await Shell.Current.GoToAsync("..");
                // }
            }
            catch (DbUpdateException dbEx) // Catch specific EF Core update exceptions
            {
                 Debug.WriteLine($"Database error saving sensor: {dbEx.Message}");
                 Debug.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");
                 await Shell.Current.DisplayAlert("Database Error", $"Failed to save sensor due to a database issue: {dbEx.InnerException?.Message ?? dbEx.Message}", "OK");
            }
            catch (Exception ex) // Catch general exceptions
            {
                Debug.WriteLine($"Error saving sensor: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to save sensor: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Deletes the specified sensor from the database after confirmation.
        /// </summary>
        /// <param name="sensor">The sensor to delete.</param>
        [RelayCommand]
        private async Task DeleteSensorAsync(Sensor? sensor)
        {
            if (sensor == null)
            {
                 Debug.WriteLine("DeleteSensorAsync called with null sensor.");
                 return;
            }

            // Confirmation dialog
            bool confirm = await Shell.Current.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete sensor '{sensor.SensorName}' (ID: {sensor.SensorId})?",
                "Yes", "No");

            if (!confirm) return;

            try
            {
                // Find the sensor in the context to ensure it's tracked before removal
                 Debug.WriteLine($"Attempting to find sensor ID {sensor.SensorId} for deletion.");
                var sensorToDelete = await _context.Sensors.FindAsync(sensor.SensorId);

                if (sensorToDelete != null)
                {
                    _context.Sensors.Remove(sensorToDelete);
                    await _context.SaveChangesAsync();
                    Debug.WriteLine($"Sensor ID {sensor.SensorId} deleted successfully.");
                    Sensors.Remove(sensor); // Remove from the observable collection
                    if (SelectedSensor == sensor)
                    {
                        PrepareNewSensor(); // Clear selection if the deleted item was selected
                    }
                    await Shell.Current.DisplayAlert("Success", "Sensor deleted successfully.", "OK");
                }
                else
                {
                     Debug.WriteLine($"Error: Sensor ID {sensor.SensorId} not found for deletion.");
                    await Shell.Current.DisplayAlert("Error", "Sensor not found.", "OK");
                }
            }
            catch (DbUpdateException dbEx) // Handle potential FK constraint issues etc.
            {
                 Debug.WriteLine($"Database error deleting sensor: {dbEx.Message}");
                 Debug.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");
                await Shell.Current.DisplayAlert("Database Error", $"Failed to delete sensor: {dbEx.InnerException?.Message ?? dbEx.Message}. It might be in use.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting sensor: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to delete sensor: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Navigates to a hypothetical Sensor Alerts page for the given sensor.
        /// </summary>
        /// <param name="sensor">The sensor whose alerts are to be viewed.</param>
        [RelayCommand]
        private async Task NavigateToAlertsAsync(Sensor sensor)
        {
            if (sensor == null) return;
            // Placeholder: Implement navigation to an alerts page if it exists
            Debug.WriteLine($"Navigate to alerts for Sensor ID: {sensor.SensorId}");
            // Example: await Shell.Current.GoToAsync($"{nameof(SensorAlertsPage)}?SensorId={sensor.SensorId}");
            await Shell.Current.DisplayAlert("Not Implemented", "Navigation to Sensor Alerts page is not yet implemented.", "OK");
        }

        /// <summary>
        /// Navigates to the firmware update page.
        /// </summary>
        [RelayCommand]
        private async Task NavigateToFirmwareUpdateAsync()
        {
            // Placeholder: Implement navigation to a firmware update page
             Debug.WriteLine("Navigating to Firmware Update page.");
             // Example: await Shell.Current.GoToAsync(nameof(FirmwareUpdatePage));
             await Shell.Current.DisplayAlert("Not Implemented", "Navigation to Firmware Update page is not yet implemented.", "OK");
        }
    }
}