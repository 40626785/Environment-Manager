namespace EnvironmentManager.Services;

using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using System.Data;

/// <summary>
/// Allows running operations on Main Thread, required by UI actions
/// 
/// Implements IRunOnMainThread to enable Dependency Injection 
/// </summary>
public class SensorThresholdService : ISensorThresholdService
{
    private IEnumerable<IThresholdRules<Sensor>> _thresholdRules;
    
    //Passed list of all implementations of IThresholdRules via Dependency Injection
    public SensorThresholdService(IEnumerable<IThresholdRules<Sensor>> thresholdRules)
    {
        _thresholdRules = thresholdRules;
    }

    /// <summary>
    /// Creates SensorThresholdBreach objects per sensor contain all breached rules
    /// </summary>
    /// <param name="sensors">Sensors to check</param>
    /// <returns>List of breaches</returns>
    public List<SensorThresholdBreach> ReturnBreached(List<Sensor> sensors)
    {
        List<SensorThresholdBreach> breached = new List<SensorThresholdBreach>();
        foreach (Sensor sensor in sensors)
        {
            List<IThresholdRules<Sensor>> rules = _thresholdRules.Where(rule => rule.IsBreachedBy(sensor)).ToList();
            breached.Add(new SensorThresholdBreach(sensor, sensor.Location, rules));
        }
        return breached;
    }
}
