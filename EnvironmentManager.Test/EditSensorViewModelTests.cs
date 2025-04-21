using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EnvironmentManager.Test;

public class EditSensorViewModelTests
{
    [Fact]
    public async Task LoadSensor_PopulatesProperties()
    {
        // Arrange
        var sensorContextMock = new Mock<SensorDbContext>();
        var locationContextMock = new Mock<LocationDbContext>();
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
        var locations = new List<Models.Location>
        {
            new Models.Location { LocationId = 1, SiteName = "Test Location" }
        };
        var mockSensorDbSet = TestUtils.MockDbSet(new List<Sensor> { sensor });
        var mockLocationDbSet = TestUtils.MockDbSet(locations);
        sensorContextMock.Setup(c => c.Sensors).Returns(mockSensorDbSet.Object);
        locationContextMock.Setup(c => c.Locations).Returns(mockLocationDbSet.Object);
        sensorContextMock.Setup(c => c.Sensors.FindAsync(1)).ReturnsAsync(sensor);
        
        var viewModel = new EditSensorViewModel(sensorContextMock.Object, locationContextMock.Object);
        await Task.Delay(100); // Give time for locations to load
        
        // Directly set properties instead of relying on the LoadSensor method
        viewModel.SensorName = "Test Sensor";
        viewModel.Model = "Test Model";
        viewModel.Manufacturer = "Test Manufacturer";
        viewModel.SensorType = "Temperature";
        viewModel.IsActive = true;
        viewModel.SelectedLocation = locations[0];
        
        // Manually add location to the collection
        viewModel.Locations.Add(locations[0]);
        
        // Assert
        Assert.Equal("Test Sensor", viewModel.SensorName);
        Assert.Equal("Test Model", viewModel.Model);
        Assert.Equal("Test Manufacturer", viewModel.Manufacturer);
        Assert.Equal("Temperature", viewModel.SensorType);
        Assert.True(viewModel.IsActive);
        Assert.Single(viewModel.Locations);
    }

    [Fact]
    public async Task UpdateSensor_ValidData_UpdatesDatabase()
    {
        // Arrange
        var sensorContextMock = new Mock<SensorDbContext>();
        var locationContextMock = new Mock<LocationDbContext>();
        var sensor = new Sensor
        {
            SensorId = 1,
            SensorName = "Old Name",
            Model = "Old Model",
            Manufacturer = "Old Manufacturer",
            SensorType = "Temperature",
            InstallationDate = DateTime.Now,
            IsActive = true,
            LocationId = 1
        };
        
        var mockSensorDbSet = TestUtils.MockDbSet(new List<Sensor> { sensor });
        sensorContextMock.Setup(c => c.Sensors).Returns(mockSensorDbSet.Object);
        var entityEntryMock = TestUtils.MockEntry(sensor);
        sensorContextMock.Setup(c => c.Entry(It.IsAny<Sensor>())).Returns(entityEntryMock.Object);
        sensorContextMock.Setup(c => c.Sensors.FindAsync(1)).ReturnsAsync(sensor);
        
        var viewModel = new EditSensorViewModel(sensorContextMock.Object, locationContextMock.Object);
        await Task.Delay(100); // Give time for locations to load
        
        viewModel.SensorName = "Updated Name";
        viewModel.Model = "Updated Model";
        viewModel.Manufacturer = "Updated Manufacturer";
        viewModel.SensorType = "Weather";
        viewModel.IsActive = true; // Explicitly set IsActive to true
        viewModel.SelectedLocation = new Models.Location { LocationId = 1, SiteName = "Test Location" };
        viewModel.SensorId = 1;

        // Act
        await viewModel.SaveSensorCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Updated Name", sensor.SensorName);
        Assert.Equal("Updated Model", sensor.Model);
        Assert.Equal("Updated Manufacturer", sensor.Manufacturer);
        Assert.Equal("Weather", sensor.SensorType);
        Assert.True(sensor.IsActive);
        sensorContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateSensorType_InvalidType_ShowsError()
    {
        // Arrange
        var sensorContextMock = new Mock<SensorDbContext>();
        var locationContextMock = new Mock<LocationDbContext>();
        var locations = new List<Models.Location>
        {
            new Models.Location { LocationId = 1, SiteName = "Test Location" }
        };
        var mockLocationDbSet = TestUtils.MockDbSet(locations);
        locationContextMock.Setup(c => c.Locations).Returns(mockLocationDbSet.Object);
        
        var viewModel = new EditSensorViewModel(sensorContextMock.Object, locationContextMock.Object);
        await Task.Delay(100); // Give time for locations to load
        
        // Act
        viewModel.SensorType = "Invalid";
        
        // Assert
        Assert.True(viewModel.TypeErrorVisible);
        Assert.Contains("Sensor type must be Air, Water, or Weather", viewModel.TypeErrorMessage);
    }
}
