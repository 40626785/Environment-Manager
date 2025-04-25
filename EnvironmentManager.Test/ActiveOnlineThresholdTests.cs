namespace EnvironmentManager.Test;

using EnvironmentManager.Models;
using EnvironmentManager.Rules;

public class ActiveOnlineThresholdTests
{
    [Fact]
    public void IsBreachedBy_OnBreached_ReturnTrue()
    {
        //Arrange
        ActiveOnlineThreshold threshold = new ActiveOnlineThreshold();
        Sensor sensor = new Sensor {IsActive = true, ConnectivityStatus = "Offline"};
        //Action
        bool result = threshold.IsBreachedBy(sensor);
        //Assert
        Assert.True(result);
    }

    [Fact]
    public void IsBreachedBy_OnInactive_ReturnFalse()
    {
        //Arrange
        ActiveOnlineThreshold threshold = new ActiveOnlineThreshold();
        Sensor sensor = new Sensor {IsActive = false, ConnectivityStatus = "Offline"};
        //Action
        bool result = threshold.IsBreachedBy(sensor);
        //Assert
        Assert.False(result);
    }

    [Fact]
    public void IsBreachedBy_OnOnline_ReturnFalse()
    {
        //Arrange
        ActiveOnlineThreshold threshold = new ActiveOnlineThreshold();
        Sensor sensor = new Sensor {IsActive = true, ConnectivityStatus = "Online"};
        //Action
        bool result = threshold.IsBreachedBy(sensor);
        //Assert
        Assert.False(result);
    }

    [Fact]
    public void ThresholdDetail_ReturnValidString()
    {
        //Arrange
        ActiveOnlineThreshold threshold = new ActiveOnlineThreshold();
        //Action
        string result = threshold.ThresholdDetail();
        //Assert
        //Simply checking the validity of the string method avoids unit test overfitting, resulting in a refactor any time the return value is changed.
        Assert.IsType<string>(result);
        Assert.NotEmpty(result);
    }
}