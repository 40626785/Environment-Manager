namespace EnvironmentManager.Test;

using EnvironmentManager.Models;
using EnvironmentManager.Rules;

public class BatteryPercentageThresholdTests
{
    [Fact]
    public void IsBreachedBy_OnBreached_ReturnTrue()
    {
        //Arrange
        BatteryPercentageThreshold threshold = new BatteryPercentageThreshold(10);
        Sensor sensor = new Sensor {BatteryLevelPercentage = 9};
        //Action
        bool result = threshold.IsBreachedBy(sensor);
        //Assert
        Assert.True(result);
    }

    [Fact]
    public void IsBreachedBy_OnNotBreached_ReturnFalse()
    {
        //Arrange
        BatteryPercentageThreshold threshold = new BatteryPercentageThreshold(10);
        Sensor sensor = new Sensor {BatteryLevelPercentage = 10};
        //Action
        bool result = threshold.IsBreachedBy(sensor);
        //Assert
        Assert.False(result);
    }

    [Fact]
    public void ThresholdDetail_ReturnValidString()
    {
        //Arrange
        BatteryPercentageThreshold threshold = new BatteryPercentageThreshold(10);
        //Action
        string result = threshold.ThresholdDetail();
        //Assert
        //Simply checking the validity of the string method avoids unit test overfitting, resulting in a refactor any time the return value is changed.
        Assert.IsType<string>(result);
        Assert.NotEmpty(result);
    }
}