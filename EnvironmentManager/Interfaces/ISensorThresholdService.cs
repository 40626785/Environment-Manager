using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

/// <summary>
/// Enables Dependency Inversion Principle, allowing verification of threshold rules without directly depending on concrete implementation
/// 
/// Implementation of Interface decided in MauiProgram.cs
/// </summary>
public interface ISensorThresholdService
{
    public List<SensorThresholdBreach> ReturnBreached(List<Sensor> sensorList);
}
