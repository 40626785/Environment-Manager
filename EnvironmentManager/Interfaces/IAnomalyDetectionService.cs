using System.Collections.Generic;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces
{
    /// <summary>
    /// Provides methods for analyzing sensor data and detecting anomalies.
    /// </summary>
    public interface IAnomalyDetectionService
    {
        List<SensorAnomaly> DetectAnomalies();
    }
}
