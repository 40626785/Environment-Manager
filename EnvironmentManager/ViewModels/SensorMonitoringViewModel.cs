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
    public partial class SensorMonitoringViewModel : ObservableObject
    {
        private readonly SensorDbContext? _context;
        private bool _isLoading;
        private CancellationTokenSource? _refreshCancellationTokenSource;
        private const int RefreshIntervalSeconds = 30;

        [ObservableProperty]
        private ObservableCollection<SensorStatus> _sensorStatuses;

        [ObservableProperty]
        private ObservableCollection<Sensor> _sensors;

        [ObservableProperty]
        private SensorStatus? _selectedSensorStatus;

        [ObservableProperty]
        private string _filterBy = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private DateTime _lastRefreshTime = DateTime.Now;

        [ObservableProperty]
        private int _totalSensors;

        [ObservableProperty]
        private int _onlineSensors;

        [ObservableProperty]
        private int _offlineSensors;

        [ObservableProperty]
        private int _degradedSensors;

        [ObservableProperty]
        private int _maintenanceSensors;

        [ObservableProperty]
        private bool _autoRefreshEnabled = true;

        // Default constructor required for XAML instantiation
        public SensorMonitoringViewModel()
        {
            _sensorStatuses = new ObservableCollection<SensorStatus>();
            _sensors = new ObservableCollection<Sensor>();
        }

        public SensorMonitoringViewModel(SensorDbContext context)
        {
            _context = context;
            _sensorStatuses = new ObservableCollection<SensorStatus>();
            _sensors = new ObservableCollection<Sensor>();
            
            // Start auto-refresh when created
            StartAutoRefresh();
            
            // Initial load
            LoadSensorStatusesCommand.Execute(null);
        }

        ~SensorMonitoringViewModel()
        {
            // Ensure we clean up resources
            StopAutoRefresh();
        }

        [RelayCommand]
        private async Task LoadSensorStatusesAsync()
        {
            if (_isLoading) return;
            try
            {
                _isLoading = true;
                IsRefreshing = true;
                Debug.WriteLine("Loading sensor statuses...");
                
                // Clear collections
                SensorStatuses.Clear();
                Sensors.Clear();
                
                // Check if context is available
                if (_context == null)
                {
                    if (Shell.Current != null)
                    {
                        await Shell.Current.DisplayAlert("Error", "Database context is not initialized", "OK");
                    }
                    return;
                }

                // Load all sensors first
                var allSensors = await _context.Sensors
                    .AsNoTracking()
                    .ToListAsync();
                
                foreach (var sensor in allSensors)
                {
                    Sensors.Add(sensor);
                }
                
                TotalSensors = allSensors.Count;

                // Load latest status for each sensor
                var latestStatuses = await _context.SensorStatuses
                    .Include(s => s.Sensor)
                    .AsNoTracking()
                    .GroupBy(s => s.SensorId)
                    .Select(g => g.OrderByDescending(s => s.StatusTimestamp).First())
                    .ToListAsync();
                
                foreach (var status in latestStatuses)
                {
                    SensorStatuses.Add(status);
                }
                
                // Update dashboard metrics
                OnlineSensors = SensorStatuses.Count(s => s.ConnectivityStatus == "Online");
                OfflineSensors = SensorStatuses.Count(s => s.ConnectivityStatus == "Offline");
                DegradedSensors = SensorStatuses.Count(s => s.ConnectivityStatus == "Degraded");
                MaintenanceSensors = SensorStatuses.Count(s => s.ConnectivityStatus == "Maintenance");
                
                // Update timestamp
                LastRefreshTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to load sensor statuses: {ex.Message}", "OK");
                }
            }
            finally
            {
                _isLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private void StartAutoRefresh()
        {
            StopAutoRefresh();
            
            if (AutoRefreshEnabled)
            {
                _refreshCancellationTokenSource = new CancellationTokenSource();
                Task.Run(async () => await AutoRefreshAsync(_refreshCancellationTokenSource.Token));
            }
        }
        
        [RelayCommand]
        private void StopAutoRefresh()
        {
            _refreshCancellationTokenSource?.Cancel();
            _refreshCancellationTokenSource?.Dispose();
            _refreshCancellationTokenSource = null;
        }
        
        private async Task AutoRefreshAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(RefreshIntervalSeconds * 1000, cancellationToken);
                    
                    if (!cancellationToken.IsCancellationRequested && AutoRefreshEnabled)
                    {
                        await LoadSensorStatusesAsync();
                    }
                }
                catch (TaskCanceledException)
                {
                    // Task was canceled, breaking loop
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Auto-refresh error: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ViewSensorDetailsAsync(SensorStatus status)
        {
            if (status == null) return;
            
            // Navigate to sensor details page, passing the sensor ID
            var parameters = new Dictionary<string, object>
            {
                { "id", status.SensorId }
            };
            
            await Shell.Current.GoToAsync(nameof(Views.EditSensorPage), parameters);
        }
        
        [RelayCommand]
        private async Task RefreshNowAsync()
        {
            await LoadSensorStatusesAsync();
        }
        
        [RelayCommand]
        private void ToggleAutoRefresh()
        {
            AutoRefreshEnabled = !AutoRefreshEnabled;
            
            if (AutoRefreshEnabled)
            {
                StartAutoRefresh();
            }
            else
            {
                StopAutoRefresh();
            }
        }
        
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