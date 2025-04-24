namespace EnvironmentManager.Test;

using EnvironmentManager.Models;
using EnvironmentManager.Services;
using EnvironmentManager.Interfaces;
using Moq;

public class SensorThresholdServiceTests
{
    [Fact]
    public async Task ReturnBreached_PopulateList()
    {
        //Arrange

        var breachedRule = new Mock<IThresholdRules<Sensor>>();
        var controlRule = new Mock<IThresholdRules<Sensor>>();
        Location location = new Location
        {
            Longitude = -3.2142174,
            Latitude = 55.9331735
        };
        Sensor breachingSensor = new Sensor 
        {
            Location = location
        };
        Sensor controlSensor = new Sensor
        {
            Location = location
        };
        breachedRule.Setup(x => x.IsBreachedBy(breachingSensor)).Returns(true);
        breachedRule.Setup(x => x.IsBreachedBy(controlSensor)).Returns(false);
        controlRule.Setup(x => x.IsBreachedBy(It.IsAny<Sensor>())).Returns(false);
        List<IThresholdRules<Sensor>> ruleList = new List<IThresholdRules<Sensor>>{breachedRule.Object, controlRule.Object};
        List<Sensor> sensorList = new List<Sensor>{breachingSensor, controlSensor};
        SensorThresholdService sensorThresholdService = new SensorThresholdService(ruleList);
        //Action
        List<SensorThresholdBreach> breaches = sensorThresholdService.ReturnBreached(sensorList);
        //Assert
        Assert.Single(breaches);
        Assert.Single(breaches[0].BreachedRules);
        Assert.Equal(breachingSensor,breaches[0].BreachingSensor);
        Assert.Equal(breachedRule.Object,breaches[0].BreachedRules[0]);
    }
}