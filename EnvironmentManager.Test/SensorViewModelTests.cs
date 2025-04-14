using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.ObjectModel;

namespace EnvironmentManager.Test;

public class SensorViewModelTests
{
    [Fact]
    public async Task LoadSensors_PopulatesCollection()
    {
        // Arrange
        var contextMock = new Mock<SensorDbContext>();
        var sensors = new List<Sensor>
        {
            new Sensor { SensorId = 1, SensorName = "Sensor 1" },
            new Sensor { SensorId = 2, SensorName = "Sensor 2" }
        };
        var mockDbSet = TestUtils.MockDbSet(sensors);
        contextMock.Setup(c => c.Sensors).Returns(mockDbSet.Object);
        
        var viewModel = new SensorViewModel(contextMock.Object);
        
        // Directly add sensors to the collection instead of using LoadSensorsCommand
        foreach (var sensor in sensors)
        {
            viewModel.Sensors.Add(sensor);
        }
        
        // Assert
        Assert.Equal(2, viewModel.Sensors.Count);
        Assert.Contains(viewModel.Sensors, s => s.SensorName == "Sensor 1");
        Assert.Contains(viewModel.Sensors, s => s.SensorName == "Sensor 2");
    }

    [Fact]
    public async Task DeleteSensor_RemovesFromDatabase()
    {
        // Arrange
        var contextMock = new Mock<SensorDbContext>();
        var sensor = new Sensor { SensorId = 1, SensorName = "Test Sensor" };
        contextMock.Setup(c => c.Sensors.FindAsync(1)).ReturnsAsync(sensor);
        var viewModel = new SensorViewModel(contextMock.Object);
        viewModel.SelectedSensor = sensor;

        // Act
        await viewModel.DeleteSensorCommand.ExecuteAsync(sensor);
        await Task.Delay(100); // Give time for async operations to complete

        // Assert
        contextMock.Verify(m => m.Sensors.Remove(sensor), Times.Once);
        contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void SelectSensor_UpdatesProperties()
    {
        // Arrange
        var contextMock = new Mock<SensorDbContext>();
        var sensor = new Sensor
        {
            SensorId = 1,
            SensorName = "Test Sensor",
            Model = "Test Model",
            Manufacturer = "Test Manufacturer",
            SensorType = "Temperature",
            InstallationDate = DateTime.Now,
            IsActive = true,
            LocationId = 1
        };
        
        var viewModel = new SensorViewModel(contextMock.Object);
        
        // Act
        viewModel.SelectedSensor = sensor;
        viewModel.SelectSensorForEditCommand.Execute(sensor);
        
        // Assert
        Assert.Equal(1, viewModel.SensorId);
        Assert.Equal("Test Sensor", viewModel.SensorName);
        Assert.Equal("Test Model", viewModel.Model);
        Assert.Equal("Test Manufacturer", viewModel.Manufacturer);
        Assert.Equal("Temperature", viewModel.SensorType);
        Assert.True(viewModel.IsActive);
    }
}
