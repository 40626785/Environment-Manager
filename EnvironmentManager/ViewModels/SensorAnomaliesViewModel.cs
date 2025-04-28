using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    /// <summary>
    /// ViewModel for loading and displaying anomalies related to a single sensor.
    /// </summary>
    public partial class SensorAnomaliesViewModel : ObservableObject, IQueryAttributable, IErrorHandling
    {
        private readonly IAnomalyDetectionService _anomalyService;
        private readonly SensorDbContext _context;

        private int _sensorId;

        [ObservableProperty]
        private ObservableCollection<SensorAnomaly> sensorAnomalies = new();

        [ObservableProperty]
        private string sensorName = string.Empty;

        [ObservableProperty]
        private string connectivityStatus = string.Empty;

        [ObservableProperty]
        private string displayError = string.Empty;

        /// <summary>
        /// Initializes a new instance of the SensorAnomaliesViewModel class.
        /// </summary>
        public SensorAnomaliesViewModel(IAnomalyDetectionService anomalyService, SensorDbContext context)
        {
            _anomalyService = anomalyService;
            _context = context;
        }

        /// <summary>
        /// Receives query parameters to identify which sensor to load anomalies for.
        /// </summary>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("sensorId", out var id) &&
                query.TryGetValue("sensorName", out var name) &&
                id is int sensorId &&
                name is string sensorName)
            {
                _sensorId = sensorId;
                SensorName = sensorName;

                LoadSensorAnomaliesAsync();
            }
        }

        /// <summary>
        /// Loads anomalies for the specified sensor, based on battery level and connectivity.
        /// </summary>
        public async Task LoadSensorAnomaliesAsync()
        {
            try
            {
                if (_sensorId == 0) return;

                var sensor = await _context.Sensors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SensorId == _sensorId);

                if (sensor == null)
                {
                    SensorAnomalies.Clear();
                    return;
                }

                var now = DateTime.Now;
                var anomalies = new ObservableCollection<SensorAnomaly>();

                if (sensor.BatteryLevelPercentage < 30)
                {
                    anomalies.Add(new SensorAnomaly
                    {
                        SensorId = sensor.SensorId,
                        SensorName = sensor.SensorName,
                        Details = "Battery level is low",
                        AnomalyType = "Low Battery",
                        DetectedAt = now
                    });
                }

                if (sensor.ConnectivityStatus != "Online")
                {
                    anomalies.Add(new SensorAnomaly
                    {
                        SensorId = sensor.SensorId,
                        SensorName = sensor.SensorName,
                        Details = "Sensor is active but offline",
                        AnomalyType = "Connectivity Issue",
                        DetectedAt = now
                    });
                }

                SensorAnomalies = anomalies;
                SensorName = sensor.SensorName;
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to load sensor anomalies.");
            }
        }

        public void HandleError(Exception ex, string message)
        {
            Debug.WriteLine($"[SensorAnomaliesViewModel] {ex.Message}");
            DisplayError = message;
        }
    }
}
