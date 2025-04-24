using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;

namespace EnvironmentManager.ViewModels;

/// <summary>
/// Code-behind the login page
/// </summary>
public class ThresholdMapViewModel
{
    private ISensorDataStore _context;
    private ISensorThresholdService _sensorThresholdService;

    public ThresholdMapViewModel(ISensorDataStore context, ISensorThresholdService sensorThresholdService)
    {
        _context = context;
        _sensorThresholdService = sensorThresholdService;
    }

    /// <summary>
    /// Creates MapPins based on all stored breaches of Threshold Rules
    /// </summary>
    /// <returns>List of MapPins representing Threshold Breaches</returns>
    public List<MapPinViewModel> GetSensorBreachPins() 
    {
        List<Sensor> sensors = _context.RetrieveAll();
        List<SensorThresholdBreach> breaches = _sensorThresholdService.ReturnBreached(sensors);
        List<MapPinViewModel> pins = breaches.Select(breach => new MapPinViewModel(breach.SensorCoordinates.X, breach.SensorCoordinates.Y, ConstructBreachLabel(breach))).ToList();
        return pins;
    }

    /// <summary>
    /// Parse string containing all details of Threshold Breach
    /// </summary>
    /// <param name="breach">Threshold Breach object containing breaching sensor and rules breached</param>
    /// <returns>String for use as breach label</returns>
    private static string ConstructBreachLabel(SensorThresholdBreach breach)
    {
        string label = $"Sensor ID: {breach.BreachingSensor.SensorId}\n";
        foreach(IThresholdRules<Sensor> threshold in breach.BreachedRules)
        {
            label = label + threshold.ThresholdDetail() + "\n";
        }
        return label;
    }
}
