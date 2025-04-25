using EnvironmentManager.Interfaces;
using NetTopologySuite.Geometries;

namespace EnvironmentManager.Models;

/// <summary>
/// Represents a Theshold Breach event. Containing metadata relating to the breaching sensor and individual rules.
/// </summary>
public class SensorThresholdBreach
{
    public Sensor BreachingSensor { get; }
    public Coordinate SensorCoordinates { get; }
    public List<IThresholdRules<Sensor>> BreachedRules { get; }
    public SensorThresholdBreach(Sensor sensor, Location sensorLocation,List<IThresholdRules<Sensor>> breachedRules) 
    {
        BreachingSensor = sensor;
        BreachedRules = breachedRules;
        SensorCoordinates = new Coordinate(sensorLocation.Longitude, sensorLocation.Latitude);
    }
}
