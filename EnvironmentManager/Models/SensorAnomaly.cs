using System;

namespace EnvironmentManager.Models
{
    /// <summary>
    /// Represents an anomaly detected in a sensor's behavior or readings.
    /// </summary>
    public class SensorAnomaly
    {
        public int SensorId { get; set; }
        public string SensorName { get; set; }
        public string AnomalyType { get; set; }
        public string Details { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
