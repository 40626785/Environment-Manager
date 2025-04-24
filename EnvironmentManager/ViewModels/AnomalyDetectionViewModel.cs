using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EnvironmentManager.ViewModels
{
    /// <summary>
    /// ViewModel for displaying anomalies detected in sensor data.
    /// </summary>
    public partial class AnomalyDetectionViewModel : ObservableObject, IErrorHandling
    {
        private readonly IAnomalyDetectionService _anomalyService;

        [ObservableProperty]
        private ObservableCollection<SensorAnomaly> anomalies = new();

        [ObservableProperty]
        private string displayError = string.Empty;

        public IRelayCommand RefreshCommand { get; }

        public AnomalyDetectionViewModel(IAnomalyDetectionService anomalyService)
        {
            _anomalyService = anomalyService;
            RefreshCommand = new RelayCommand(LoadAnomalies);
            LoadAnomalies();
        }

        private void LoadAnomalies()
        {
            try
            {
                var detected = _anomalyService.DetectAnomalies();

                foreach (var anomaly in detected)
                {
                    anomaly.AnomalyType = GetAnomalyType(anomaly);
                    anomaly.DetectedAt = DateTime.Now; // Add logic if real timestamp exists
                }

                Anomalies = new ObservableCollection<SensorAnomaly>(detected);
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to load anomalies.");
            }
        }

        private string GetAnomalyType(SensorAnomaly anomaly)
        {
            if (anomaly.Details.Contains("Battery", StringComparison.OrdinalIgnoreCase))
                return "Low Battery";
            if (anomaly.Details.Contains("offline", StringComparison.OrdinalIgnoreCase))
                return "Connectivity Issue";
            return "Unknown";
        }

        public void HandleError(Exception e, string message)
        {
            Trace.WriteLine(e.Message);
            DisplayError = message;
        }
    }
}
