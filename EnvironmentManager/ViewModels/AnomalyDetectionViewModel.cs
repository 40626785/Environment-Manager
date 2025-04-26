using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EnvironmentManager.ViewModels
{
    /// <summary>
    /// ViewModel for detecting and displaying sensor anomalies across all sensors.
    /// </summary>
    public partial class AnomalyDetectionViewModel : ObservableObject, IErrorHandling
    {
        private readonly SensorDbContext _context;

        [ObservableProperty]
        private ObservableCollection<SensorAnomaly> anomalies = new();

        [ObservableProperty]
        private string displayError = string.Empty;

        /// <summary>
        /// Command to refresh and reload anomaly data.
        /// </summary>
        public IAsyncRelayCommand RefreshCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnomalyDetectionViewModel"/> class.
        /// </summary>
        /// <param name="context">The database context for sensors.</param>
        public AnomalyDetectionViewModel(SensorDbContext context)
        {
            _context = context;
            RefreshCommand = new AsyncRelayCommand(LoadAnomaliesAsync);
        }

        /// <summary>
        /// Loads anomalies detected from all sensors asynchronously.
        /// </summary>
        public async Task LoadAnomaliesAsync()
        {
            try
            {
                var allSensors = await _context.Sensors.AsNoTracking().ToListAsync();
                var allAnomalies = new ObservableCollection<SensorAnomaly>();
                var now = DateTime.Now;

                foreach (var sensor in allSensors)
                {
                    if (sensor.BatteryLevelPercentage < 30)
                    {
                        allAnomalies.Add(new SensorAnomaly
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
                        allAnomalies.Add(new SensorAnomaly
                        {
                            SensorId = sensor.SensorId,
                            SensorName = sensor.SensorName,
                            Details = "Sensor is active but offline",
                            AnomalyType = "Connectivity Issue",
                            DetectedAt = now
                        });
                    }
                }

                Anomalies = allAnomalies;
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to load anomalies.");
            }
        }

        public void HandleError(Exception e, string message)
        {
            Debug.WriteLine(e.Message);
            DisplayError = message;
        }
    }
}
