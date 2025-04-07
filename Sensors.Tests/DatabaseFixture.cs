using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Sensors.Tests;

// Mock data container for tests
public class MockData
{
    public List<Sensor> Sensors { get; } = new List<Sensor>();
    public List<SensorReading> Readings { get; } = new List<SensorReading>();
    public List<SensorSetting> Settings { get; } = new List<SensorSetting>();
}

public class SensorSetting
{
    public int SettingId { get; set; }
    public int SensorId { get; set; }
    public string SettingName { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
}

public class DatabaseFixture : IDisposable
{
    public MockData Data { get; private set; } = new MockData();
    public SqlConnection Connection { get; private set; }

    public DatabaseFixture()
    {
        // Initialize with mock data instead of real DB connection
        InitializeMockData();
        
        // Create a fake connection that won't be used but allows the test code to compile
        Connection = new SqlConnection("Server=localhost;Database=fake_db;");
    }

    private void InitializeMockData()
    {
        // Add mock sensors
        Data.Sensors.Add(new Sensor
        {
            SensorId = 1,
            SensorName = "Temperature Sensor Alpha",
            SensorType = "Thermometer",
            LocationId = 1,
            IsActive = true,
            ConnectivityStatus = "Online"
        });

        Data.Sensors.Add(new Sensor
        {
            SensorId = 2,
            SensorName = "Humidity Sensor Beta",
            SensorType = "Hygrometer",
            LocationId = 1,
            IsActive = true,
            ConnectivityStatus = "Online"
        });

        Data.Sensors.Add(new Sensor
        {
            SensorId = 3,
            SensorName = "Pressure Sensor Gamma",
            SensorType = "Barometer",
            LocationId = 2,
            IsActive = false,
            ConnectivityStatus = "Offline"
        });

        // Add mock readings for sensor 1
        Data.Readings.Add(new SensorReading
        {
            ReadingId = 1,
            SensorId = 1,
            Timestamp = DateTime.UtcNow.AddHours(-1),
            Value = 22.5f,
            MeasurementUnit = "Celsius"
        });

        Data.Readings.Add(new SensorReading
        {
            ReadingId = 2,
            SensorId = 1,
            Timestamp = DateTime.UtcNow.AddMinutes(-30),
            Value = 23.1f,
            MeasurementUnit = "Celsius"
        });

        // Add mock readings for sensor 2
        Data.Readings.Add(new SensorReading
        {
            ReadingId = 3,
            SensorId = 2,
            Timestamp = DateTime.UtcNow.AddHours(-1),
            Value = 45.2f,
            MeasurementUnit = "%"
        });

        // Add mock settings
        Data.Settings.Add(new SensorSetting
        {
            SettingId = 1,
            SensorId = 1,
            SettingName = "CalibrationOffset",
            SettingValue = "0.0"
        });
    }

    public void Dispose()
    {
        // Clean up resources if needed
        GC.SuppressFinalize(this);
    }
} 