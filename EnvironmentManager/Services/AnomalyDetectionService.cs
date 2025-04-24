using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Data;

namespace EnvironmentManager.Services
{
    /// <summary>
    /// Analyzes sensor readings to detect anomalies.
    /// Implements IAnomalyDetectionService for DI compatibility.
    /// </summary>
    public class AnomalyDetectionService : IAnomalyDetectionService
    {
        private readonly SensorDbContext _sensorContext;

        public AnomalyDetectionService(SensorDbContext sensorContext)
        {
            _sensorContext = sensorContext;
        }

        public List<SensorAnomaly> DetectAnomalies()
        {
            var anomalies = new List<SensorAnomaly>();
            var sensors = _sensorContext.Sensors.Include(s => s.Location).ToList();

            foreach (var sensor in sensors)
            {
                if (sensor.BatteryLevelPercentage.HasValue && sensor.BatteryLevelPercentage < 20)
                {
                    anomalies.Add(new SensorAnomaly
                    {
                        SensorId = sensor.SensorId,
                        SensorName = sensor.SensorName,
                        AnomalyType = "Low Battery",
                        Details = $"Battery level is {sensor.BatteryLevelPercentage}%",
                        DetectedAt = DateTime.Now
                    });
                }

                if (sensor.IsActive && sensor.ConnectivityStatus == "Offline")
                {
                    anomalies.Add(new SensorAnomaly
                    {
                        SensorId = sensor.SensorId,
                        SensorName = sensor.SensorName,
                        AnomalyType = "Connectivity Issue",
                        Details = "Sensor is active but offline",
                        DetectedAt = DateTime.Now
                    });
                }
            }

            return anomalies;
        }
    }
}
