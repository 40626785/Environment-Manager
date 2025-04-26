using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EnvironmentManager.Test;

public class FirmwareUpdateViewModelTests
{
    [Fact]
    public async Task LoadSensorsAsync_LoadsAllSensors()
    {
        // Arrange
        var sensorsList = new List<Sensor>
        {
            new Sensor { SensorId = 1, SensorName = "Sensor A" },
            new Sensor { SensorId = 2, SensorName = "Sensor B" }
        };
        var mockSensorDbSet = TestUtils.MockDbSet(sensorsList);
        var mockDbContext = new Mock<SensorDbContext>();
        mockDbContext.Setup(c => c.Sensors).Returns(mockSensorDbSet.Object);

        var viewModel = new FirmwareUpdateViewModel(mockDbContext.Object);

        // Act
        await viewModel.LoadSensorsAsync();

        // Assert
        Assert.Equal(2, viewModel.Sensors.Count);
    }

    [Fact]
    public async Task UpdateFirmwareAsync_UpdatesSelectedSensors()
    {
        // Arrange
        var sensor = new Sensor { SensorId = 1, SensorName = "Sensor A" };
        var sensorsList = new List<Sensor> { sensor };
        var mockSensorDbSet = TestUtils.MockDbSet(sensorsList);
        var mockDbContext = new Mock<SensorDbContext>();
        mockDbContext.Setup(c => c.Sensors).Returns(mockSensorDbSet.Object);
        mockDbContext.Setup(c => c.Sensors.FindAsync(1))
            .ReturnsAsync(sensor);

        var viewModel = new FirmwareUpdateViewModel(mockDbContext.Object);
        await viewModel.LoadSensorsAsync();
        viewModel.FirmwareVersion = "v1.2.3";
        viewModel.Sensors[0].IsSelected = true;

        // Act
        await viewModel.UpdateFirmwareAsync();

        // Assert
        Assert.Equal("v1.2.3", sensor.FirmwareVersion);
        Assert.Contains("updated for 1", viewModel.StatusMessage);
        mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void HandleError_StoresErrorMessage()
    {
        // Arrange
        var mockDbContext = new Mock<SensorDbContext>();
        var viewModel = new FirmwareUpdateViewModel(mockDbContext.Object);

        // Act
        viewModel.HandleError(new Exception(), "Test error");

        // Assert
        Assert.Equal("Test error", viewModel.StatusMessage);
    }
}
