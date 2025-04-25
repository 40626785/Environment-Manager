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
    public partial class AnomalyDetectionViewModel : ObservableObject, IErrorHandling
    {
        private readonly SensorDbContext _context;

        [ObservableProperty]
        private ObservableCollection<SensorAnomaly> anomalies = new();

        [ObservableProperty]
        private string displayError = string.Empty;

        public IAsyncRelayCommand RefreshCommand { get; }

        public AnomalyDetectionViewModel(SensorDbContext context)
        {
            _context = context;
            RefreshCommand = new AsyncRelayCommand(LoadAnomaliesAsync);
        }

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
                Debug.WriteLine($"[AnomalyDetection] Error: {ex.Message}");
                DisplayError = "Failed to load anomalies.";
            }
        }

        public void HandleError(Exception e, string message)
        {
            Debug.WriteLine(e.Message);
            DisplayError = message;
        }
    }
}
