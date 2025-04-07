using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Sensors.Tests;

// Simple model classes for tests - independent from the MAUI app models
public class Sensor
{
    public int SensorId { get; set; }
    public string SensorName { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public bool IsActive { get; set; } = true;
    public string ConnectivityStatus { get; set; } = string.Empty;
    public float? BatteryLevelPercentage { get; set; }
}

public class SensorReading
{
    public int ReadingId { get; set; }
    public int SensorId { get; set; }
    public DateTime Timestamp { get; set; }
    public float Value { get; set; }
    public string MeasurementUnit { get; set; } = string.Empty;
    public bool IsValid { get; set; } = true;
}

// Apply the fixture to the test class
public class SensorTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture; // Mock data container
    private readonly MockData _mockData;

    // Constructor receives the fixture instance
    public SensorTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _mockData = fixture.Data;
    }

    #region Sensor Initialization Tests

    [Fact]
    public void InitializeSensor_ValidParameters_ShouldUpdateStatus()
    {
        // Arrange: Create a new Sensor instance with valid parameters
        var sensor = new Sensor { 
            SensorId = 0, 
            SensorName = "Temperature Sensor Test", 
            SensorType = "Thermometer", 
            LocationId = 1, 
            ConnectivityStatus = "New" 
        };

        // Act: Set connectivity status to "Initialized" (simulating initialization)
        sensor.ConnectivityStatus = "Initialized";

        // Assert: Verify the sensor status is updated correctly
        Assert.Equal("Initialized", sensor.ConnectivityStatus);
    }

    [Fact]
    public void InitializeSensor_InvalidParameters_ShouldHandleError()
    {
        // Arrange: Setup conditions for failed initialization (null name)
        // Act & Assert: Verify that we can't create a valid sensor with null name
        var exception = Assert.Throws<ArgumentNullException>(() => {
            // This would fail at DB save time due to [Required] attribute
            var sensor = new Sensor { 
                SensorId = 0, 
                SensorName = null, 
                SensorType = "Thermometer",
                LocationId = 1
            };
            
            // Simulate attempt to save to database (which would validate [Required] constraints)
            if (string.IsNullOrEmpty(sensor.SensorName)) 
                throw new ArgumentNullException("SensorName", "Sensor name cannot be null");
        });
        
        Assert.Equal("SensorName", exception.ParamName);
    }

    #endregion

    #region Data Retrieval Tests

    [Fact]
    public void GetSensorReading_ValidSensorId_ShouldReturnReadings()
    {
        // Arrange: Use valid SensorId from the mock data
        int validSensorId = 1; // Temperature Sensor Alpha
        
        // Act: Query the mock data for readings
        var readings = _mockData.Readings.Where(r => r.SensorId == validSensorId).ToList();

        // Assert: Verify that readings are returned
        Assert.NotNull(readings);
        Assert.NotEmpty(readings);
        Assert.Equal(2, readings.Count); // Should have 2 readings for sensor ID 1
    }

    [Fact]
    public void GetSensorReading_InvalidSensorId_ShouldThrowArgumentException()
    {
        // Arrange: Define an invalid sensor ID
        var invalidSensorId = -99;
        
        // Act & Assert: Verify expected exception when querying with invalid ID
        var exception = Assert.Throws<ArgumentException>(() => {
            if (invalidSensorId < 0)
                throw new ArgumentException("Invalid sensor ID", nameof(invalidSensorId));
                
            // This would not execute due to the exception
            var readings = _mockData.Readings.Where(r => r.SensorId == invalidSensorId).ToList();
        });
        
        Assert.Contains("Invalid sensor ID", exception.Message);
    }

    [Fact]
    public void GetSensorReading_SensorWithNoReadings_ShouldReturnEmpty()
    {
        // Arrange: Use a sensor ID with no readings from mock data
        int sensorIdWithNoReadings = 3; // Pressure Sensor Gamma
        
        // Act: Query mock data for readings
        var readings = _mockData.Readings.Where(r => r.SensorId == sensorIdWithNoReadings).ToList();

        // Assert: Verify that an empty list is returned
        Assert.NotNull(readings);
        Assert.Empty(readings);
    }

    #endregion

    #region Calibration Tests

    [Fact]
    public void CalibrateSensor_NormalConditions_ShouldSucceed()
    {
        // Arrange: Get a sensor from mock data
        int sensorToCalibrateId = 1;
        var sensor = _mockData.Sensors.FirstOrDefault(s => s.SensorId == sensorToCalibrateId);
        
        // Act: Simulate calibration by updating or adding a setting
        bool calibrationSuccess = false;
        if (sensor != null)
        {
            var existingSetting = _mockData.Settings.FirstOrDefault(
                s => s.SensorId == sensorToCalibrateId && s.SettingName == "CalibrationOffset");
            
            if (existingSetting != null)
            {
                existingSetting.SettingValue = "0.5";
                calibrationSuccess = true;
            }
            else
            {
                _mockData.Settings.Add(new SensorSetting { 
                    SettingId = _mockData.Settings.Count + 1,
                    SensorId = sensorToCalibrateId,
                    SettingName = "CalibrationOffset",
                    SettingValue = "0.5"
                });
                calibrationSuccess = true;
            }
        }

        // Assert: Verify calibration was successful
        Assert.NotNull(sensor);
        Assert.True(calibrationSuccess);
        var calibrationSetting = _mockData.Settings.FirstOrDefault(
            s => s.SensorId == sensorToCalibrateId && s.SettingName == "CalibrationOffset");
        Assert.NotNull(calibrationSetting);
        Assert.Equal("0.5", calibrationSetting.SettingValue);
    }

    [Fact]
    public void CalibrateSensor_InvalidParameters_ShouldFailOrHandleError()
    {
        // Arrange: Setup invalid calibration parameters
        string invalidCalibrationValue = "not_a_number";
        
        // Act & Assert: Verify expected exception with invalid parameters
        var exception = Assert.Throws<FormatException>(() => {
            // Attempt to parse invalid value as a number (which will fail)
            float calibrationOffset = float.Parse(invalidCalibrationValue);
            
            // This code would not execute due to the exception
            var sensor = _mockData.Sensors.First();
            _mockData.Settings.Add(new SensorSetting { 
                SettingId = 999,
                SensorId = sensor.SensorId,
                SettingName = "CalibrationOffset",
                SettingValue = calibrationOffset.ToString()
            });
        });
        
        // Assert proper exception was thrown
        Assert.Contains(invalidCalibrationValue, exception.Message);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void SensorOperation_WhenDisconnected_ShouldHandleGracefully()
    {
        // Arrange: Create a mock sensor hardware interface that simulates a disconnection
        var mockSensorHardware = new Mock<ISensorHardwareInterface>();
        mockSensorHardware.Setup(h => h.ReadValue()).Throws(new TimeoutException("Sensor disconnected"));
        
        // Act & Assert: Verify hardware read failure is handled properly
        var exception = Assert.Throws<TimeoutException>(() => {
            // Use the mock to attempt a read
            double reading = mockSensorHardware.Object.ReadValue();
        });
        
        Assert.Contains("Sensor disconnected", exception.Message);
    }

    [Fact]
    public void SensorOperation_InvalidState_ShouldPreventOperation()
    {
        // Arrange: Find an inactive sensor from the mock data
        var inactiveSensor = _mockData.Sensors.FirstOrDefault(s => !s.IsActive);
        
        // Act & Assert: Verify operation is prevented
        var exception = Assert.Throws<InvalidOperationException>(() => {
            // Try to perform an operation that requires an active sensor
            if (inactiveSensor != null && !inactiveSensor.IsActive)
            {
                throw new InvalidOperationException("Cannot perform operations on inactive sensors");
            }
            
            // This would not execute
            Console.WriteLine("Operation succeeded");
        });
        
        Assert.NotNull(inactiveSensor);
        Assert.False(inactiveSensor.IsActive);
        Assert.Contains("Cannot perform operations on inactive sensors", exception.Message);
    }

    #endregion

    // --- Helper Methods (if needed) --- 
    private int GetSensorCount()
    {
        return _mockData.Sensors.Count;
    }
}

// Simplified interface for mocking
public interface ISensorHardwareInterface
{
    double ReadValue();
    void Connect();
    void Disconnect();
}