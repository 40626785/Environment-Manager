using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace EnvironmentManager.ViewModels
{
    /// <summary>
    /// ViewModel for the Sensor Monitoring page, displaying the latest status of all sensors
    /// and providing summary metrics. Supports auto-refresh functionality.
    /// </summary>
    public partial class SensorMonitoringViewModel : ObservableObject
    {
        private readonly SensorDbContext? _context; // Nullable context to support parameterless constructor
        private bool _isLoading; // Flag to prevent concurrent loading
        private CancellationTokenSource? _refreshCancellationTokenSource; // Used to cancel the auto-refresh task
        private const int RefreshIntervalSeconds = 30; // Interval for auto-refresh

        /// <summary>
        /// Collection of the latest status for each sensor.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SensorStatus> _sensorStatuses;

        /// <summary>
        /// Collection of all sensors (used for filtering or potential future use).
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Sensor> _sensors;

        /// <summary>
        /// The currently selected sensor status in the UI.
        /// </summary>
        [ObservableProperty]
        private SensorStatus? _selectedSensorStatus;

        /// <summary>
        /// Search or filter text entered by the user (not currently implemented in LoadSensorStatusesAsync).
        /// </summary>
        [ObservableProperty]
        private string _filterBy = string.Empty;

        /// <summary>
        /// Indicates if a refresh operation is currently in progress.
        /// </summary>
        [ObservableProperty]
        private bool _isRefreshing;

        /// <summary>
        /// Timestamp of the last successful refresh.
        /// </summary>
        [ObservableProperty]
        private DateTime _lastRefreshTime = DateTime.Now;

        // Dashboard Metrics
        /// <summary>
        /// Total number of sensors.
        /// </summary>
        [ObservableProperty]
        private int _totalSensors;

        /// <summary>
        /// Number of sensors currently online.
        /// </summary>
        [ObservableProperty]
        private int _onlineSensors;

        /// <summary>
        /// Number of sensors currently offline.
        /// </summary>
        [ObservableProperty]
        private int _offlineSensors;

        /// <summary>
        /// Number of sensors with a degraded status.
        /// </summary>
        [ObservableProperty]
        private int _degradedSensors;

        /// <summary>
        /// Number of sensors currently marked as under maintenance.
        /// </summary>
        [ObservableProperty]
        private int _maintenanceSensors;

        /// <summary>
        /// Gets or sets a value indicating whether auto-refresh is enabled.
        /// </summary>
        [ObservableProperty]
        private bool _autoRefreshEnabled = true;

        /// <summary>
        /// Default constructor for XAML instantiation. Should ideally not be used directly if context is required.
        /// </summary>
        public SensorMonitoringViewModel()
        {
            _sensorStatuses = new ObservableCollection<SensorStatus>();
            _sensors = new ObservableCollection<Sensor>();
            // Warning: Context will be null here. LoadSensorStatusesCommand might fail if called.
            Debug.WriteLine("SensorMonitoringViewModel created with parameterless constructor - DbContext is null.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorMonitoringViewModel"/> class with database context.
        /// </summary>
        /// <param name="context">The database context for sensor data.</param>
        public SensorMonitoringViewModel(SensorDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _sensorStatuses = new ObservableCollection<SensorStatus>();
            _sensors = new ObservableCollection<Sensor>();
            
            // Start auto-refresh background task
            StartAutoRefresh();
            
            // Perform initial data load
            LoadSensorStatusesCommand.Execute(null);
        }

        /// <summary>
        /// Finalizer to ensure cancellation token source is disposed.
        /// </summary>
        ~SensorMonitoringViewModel()
        {
            StopAutoRefresh();
        }

        /// <summary>
        /// Asynchronously loads the latest status for each sensor from the database.
        /// Updates the sensor collections and dashboard metrics.
        /// </summary>
        [RelayCommand]
        private async Task LoadSensorStatusesAsync()
        {
            if (_isLoading) return; // Prevent overlapping refreshes
            try
            {
                _isLoading = true;
                IsRefreshing = true;
                Debug.WriteLine("Loading sensor statuses...");
                
                if (_context == null)
                {
                    Debug.WriteLine("Error: Database context is null in LoadSensorStatusesAsync.");
                    // Optionally display user-friendly error or rely on initial DI check
                    // if (Shell.Current != null) { await Shell.Current.DisplayAlert("Error", "Database context is not available", "OK"); }
                    return;
                }

                // Clear existing data before loading new data
                SensorStatuses.Clear();
                Sensors.Clear(); // Also clearing this, though it might be populated differently depending on use case
                
                // Load all sensors to get the total count and potentially populate the Sensors collection
                // Consider if loading *all* sensor details is necessary or just IDs/names for matching statuses
                var allSensors = await _context.Sensors
                    .AsNoTracking()
                    .ToListAsync();
                
                // Populate the observable collection if needed elsewhere, otherwise just get the count
                // foreach (var sensor in allSensors)
                // {
                //     Sensors.Add(sensor);
                // }
                TotalSensors = allSensors.Count;

                // Efficiently get the latest status entry for each sensor using GroupBy and Select
                var latestStatuses = await _context.SensorStatuses
                    .Include(s => s.Sensor) // Include sensor details for display
                    .AsNoTracking()
                    .GroupBy(s => s.SensorId) // Group statuses by sensor
                    .Select(g => g.OrderByDescending(s => s.StatusTimestamp).First()) // Select the latest status in each group
                    .ToListAsync();
                
                // Populate the main collection bound to the UI
                foreach (var status in latestStatuses)
                {
                    SensorStatuses.Add(status);
                }
                Debug.WriteLine($"Loaded {SensorStatuses.Count} latest sensor statuses.");
                
                // Calculate and update dashboard metrics based on the latest statuses
                OnlineSensors = SensorStatuses.Count(s => s.ConnectivityStatus == "Online");
                OfflineSensors = SensorStatuses.Count(s => s.ConnectivityStatus == "Offline");
                DegradedSensors = SensorStatuses.Count(s => s.ConnectivityStatus == "Degraded");
                MaintenanceSensors = SensorStatuses.Count(s => s.ConnectivityStatus == "Maintenance");
                
                // Update the last refresh timestamp
                LastRefreshTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading sensor statuses: {ex.Message}");
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to load sensor statuses: {ex.Message}", "OK");
                }
            }
            finally
            {
                _isLoading = false;
                IsRefreshing = false; // Ensure refreshing indicator is turned off
            }
        }

        /// <summary>
        /// Starts the background task for auto-refreshing sensor statuses.
        /// </summary>
        [RelayCommand]
        private void StartAutoRefresh()
        {
            // Ensure any existing task is stopped before starting a new one
            StopAutoRefresh();
            
            if (AutoRefreshEnabled)
            {
                Debug.WriteLine("Starting auto-refresh task.");
                _refreshCancellationTokenSource = new CancellationTokenSource();
                // Run the refresh loop in a background thread
                Task.Run(async () => await AutoRefreshLoopAsync(_refreshCancellationTokenSource.Token));
            }
        }
        
        /// <summary>
        /// Stops the background auto-refresh task.
        /// </summary>
        [RelayCommand]
        private void StopAutoRefresh()
        {
             if (_refreshCancellationTokenSource != null && !_refreshCancellationTokenSource.IsCancellationRequested)
             {
                 Debug.WriteLine("Stopping auto-refresh task.");
                 _refreshCancellationTokenSource.Cancel();
                 _refreshCancellationTokenSource.Dispose();
                 _refreshCancellationTokenSource = null;
             }
        }
        
        // The core loop for the auto-refresh background task
        private async Task AutoRefreshLoopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Auto-refresh loop started. Interval: {RefreshIntervalSeconds} seconds.");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Wait for the specified interval or until cancellation is requested
                    await Task.Delay(TimeSpan.FromSeconds(RefreshIntervalSeconds), cancellationToken);
                    
                    // Check again for cancellation after the delay
                    if (!cancellationToken.IsCancellationRequested && AutoRefreshEnabled)
                    {
                        Debug.WriteLine("Auto-refresh triggered. Loading statuses...");
                        // Execute the load command on the main thread if it updates UI-bound collections
                         MainThread.BeginInvokeOnMainThread(async () => await LoadSensorStatusesAsync());
                    }
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("Auto-refresh task canceled.");
                    break; // Exit loop if task is canceled
                }
                catch (Exception ex)
                {
                    // Log errors during auto-refresh but allow the loop to continue
                    Debug.WriteLine($"Error during auto-refresh loop: {ex.Message}");
                    // Consider adding a delay here to prevent rapid error logging
                    await Task.Delay(TimeSpan.FromSeconds(RefreshIntervalSeconds), CancellationToken.None); // Wait before retrying
                }
            }
             Debug.WriteLine("Auto-refresh loop ended.");
        }
        
        /// <summary>
        /// Navigates to the sensor details/edit page for the selected sensor status.
        /// </summary>
        /// <param name="status">The selected sensor status.</param>
        [RelayCommand]
        private async Task ViewSensorDetailsAsync(SensorStatus? status)
        {
            if (status == null) 
            {
                 Debug.WriteLine("ViewSensorDetailsAsync called with null status.");
                 return;
            }
            
            // Navigate to the EditSensorPage, passing the SensorId
             Debug.WriteLine($"Navigating to details for Sensor ID: {status.SensorId}");
            var parameters = new Dictionary<string, object>
            {
                { "id", status.SensorId }
            };
            
            await Shell.Current.GoToAsync(nameof(Views.EditSensorPage), parameters);
        }
        
        /// <summary>
        /// Manually triggers an immediate refresh of the sensor statuses.
        /// </summary>
        [RelayCommand]
        private async Task RefreshNowAsync()
        {
             Debug.WriteLine("Manual refresh requested.");
            await LoadSensorStatusesAsync();
        }
        
        /// <summary>
        /// Toggles the auto-refresh functionality on or off.
        /// </summary>
        [RelayCommand]
        private void ToggleAutoRefresh()
        {
            // Property change will trigger Start/Stop via OnAutoRefreshEnabledChanged
            AutoRefreshEnabled = !AutoRefreshEnabled;
            Debug.WriteLine($"Auto-refresh toggled. Enabled: {AutoRefreshEnabled}");
        }
        
        // Automatically start/stop the refresh task when the property changes
        partial void OnAutoRefreshEnabledChanged(bool value)
        {
            if (value)
            {
                StartAutoRefresh();
            }
            else
            {
                StopAutoRefresh();
            }
        }
    }
} 