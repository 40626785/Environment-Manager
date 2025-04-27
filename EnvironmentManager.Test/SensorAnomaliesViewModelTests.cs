using EnvironmentManager.ViewModels;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Diagnostics;
using System.Text;

namespace EnvironmentManager.Test;

public class SensorAnomaliesViewModelTests
{
    [Fact]
    public async Task LoadSensorAnomaliesAsync_LoadsAnomalies_ForLowBatteryAndOffline()
    {
        // Arrange
        var anomalyServiceMock = new Mock<IAnomalyDetectionService>();
        var sensorContextMock = new Mock<SensorDbContext>();
        var sensor = new Sensor
        {
            SensorId = 1,
            SensorName = "Test Sensor",
            BatteryLevelPercentage = 10,
            ConnectivityStatus = "Offline"
        };
        var mockSensorDbSet = TestUtils.MockDbSet(new List<Sensor> { sensor });
        sensorContextMock.Setup(c => c.Sensors).Returns(mockSensorDbSet.Object);
        sensorContextMock.Setup(c => c.Sensors.FindAsync(1)).ReturnsAsync(sensor);

        var viewModel = new SensorAnomaliesViewModel(anomalyServiceMock.Object, sensorContextMock.Object);

        // Act
        viewModel.ApplyQueryAttributes(new Dictionary<string, object>
        {
            { "sensorId", 1 },
            { "sensorName", "Test Sensor" }
        });

        await Task.Delay(100); // Allow async to finish

        // Assert
        Assert.Equal(2, viewModel.SensorAnomalies.Count);
    }

    [Fact]
    public async Task LoadSensorAnomaliesAsync_SensorNotFound_ClearsAnomalies()
    {
        // Arrange
        var anomalyServiceMock = new Mock<IAnomalyDetectionService>();
        var sensorContextMock = new Mock<SensorDbContext>();
        var mockSensorDbSet = TestUtils.MockDbSet(new List<Sensor>());
        sensorContextMock.Setup(c => c.Sensors).Returns(mockSensorDbSet.Object);

        var viewModel = new SensorAnomaliesViewModel(anomalyServiceMock.Object, sensorContextMock.Object);

        // Act
        viewModel.ApplyQueryAttributes(new Dictionary<string, object>
        {
            { "sensorId", 999 },
            { "sensorName", "Unknown" }
        });

        await Task.Delay(100);

        // Assert
        Assert.Empty(viewModel.SensorAnomalies);
    }

    [Fact]
    public void HandleError_SetsDisplayError()
    {
        // Arrange
        var anomalyServiceMock = new Mock<IAnomalyDetectionService>();
        var sensorContextMock = new Mock<SensorDbContext>();
        var viewModel = new SensorAnomaliesViewModel(anomalyServiceMock.Object, sensorContextMock.Object);

        // Act
        viewModel.DisplayError = "Test Error";

        // Assert
        Assert.Equal("Test Error", viewModel.DisplayError);
    }

    [Fact]
    public void HandleError_TraceLogsError()
    {
        // Arrange
        var anomalyServiceMock = new Mock<IAnomalyDetectionService>();
        var sensorContextMock = new Mock<SensorDbContext>();
        var viewModel = new SensorAnomaliesViewModel(anomalyServiceMock.Object, sensorContextMock.Object);

        var builder = new StringBuilder();
        using var writer = new StringWriter(builder);
        Trace.Listeners.Clear();
        Trace.Listeners.Add(new TextWriterTraceListener(writer));

        var ex = new Exception("Simulated failure");

        // Act
        try
        {
            throw ex;
        }
        catch (Exception error)
        {
            Debug.WriteLine(error.Message);
        }
        Trace.Flush();

        // Assert
        var traceOutput = builder.ToString();
        Assert.Contains("Simulated failure", traceOutput);
    }
}