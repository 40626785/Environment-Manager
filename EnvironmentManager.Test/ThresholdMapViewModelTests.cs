namespace EnvironmentManager.Test;

using EnvironmentManager.Models;
using EnvironmentManager.Services;
using EnvironmentManager.Interfaces;
using Moq;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Rules;
using NetTopologySuite.Geometries;

public class ThresholdMapViewModelTests
{
    [Fact]
    public void GetSensorBreachPins_PopulateList()
    {
        //Arrange
        //Setup sensors
        Models.Location location1 = new Models.Location{Longitude = 35.4444, Latitude = 30.3285};
        Models.Location location2 = new Models.Location{Longitude = 72.5450, Latitude =  13.1631};
        Sensor sensor1 = new Sensor {Location = location1};
        Sensor sensor2 = new Sensor {Location = location2};
        List<Sensor> sensors = new List<Sensor>{sensor1, sensor2};
        //Setup lists of breached rules
        int batteryThreshold1 = 1;
        int batteryThreshold2 = 10;
        List<IThresholdRules<Sensor>> ruleBreaches1 = new List<IThresholdRules<Sensor>>{new BatteryPercentageThreshold(batteryThreshold1)};
        List<IThresholdRules<Sensor>> ruleBreaches2 = new List<IThresholdRules<Sensor>>{new BatteryPercentageThreshold(batteryThreshold2), new ActiveOnlineThreshold()};
        //Setup breaches
        SensorThresholdBreach breach1 = new SensorThresholdBreach(sensor1, location1, ruleBreaches1);
        SensorThresholdBreach breach2 = new SensorThresholdBreach(sensor2, location2, ruleBreaches2);
        List<SensorThresholdBreach> breaches = new List<SensorThresholdBreach>{breach1, breach2};
        //Mock SensorDataStore
        var dataStore = new Mock<ISensorDataStore>();
        dataStore.Setup(x => x.RetrieveAll()).Returns(sensors);
        //Mock SensorThresholdService
        var thresholdService = new Mock<ISensorThresholdService>();
        thresholdService.Setup(x => x.ReturnBreached(sensors)).Returns(breaches);
        ThresholdMapViewModel viewModel = new ThresholdMapViewModel(dataStore.Object, thresholdService.Object);
        //Action
        List<MapPinViewModel> result = viewModel.GetSensorBreachPins();
        //Assert
        Assert.Equal(breaches.Count, result.Count); //assert there is a pin for each breach
        //assert map pins contain correct metadata
        for(int count = 0; count < result.Count; count++)
        {
            Assert.Equal(breaches[count].SensorCoordinates, new Coordinate(result[count].Longitude, result[count].Latitude));
            foreach(var breach in breaches[count].BreachedRules)
            {
                Assert.Contains(breach.ThresholdDetail(), result[count].Label);
            }
        }
    }
}