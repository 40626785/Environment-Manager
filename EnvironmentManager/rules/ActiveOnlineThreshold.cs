namespace EnvironmentManager.Rules;

using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;

/// <summary>
/// Rule to verify that sensor is not offline when in active status
/// </summary>
public class ActiveOnlineThreshold : IThresholdRules<Sensor>
{   
   /// <summary>
   /// Checks if sensor is active but not online
   /// </summary>
   /// <param name="sensor">Object to check status of</param>
   /// <returns>If active but not online</returns>
   public bool IsBreachedBy(Sensor sensor)
   {
        return sensor.IsActive && sensor.ConnectivityStatus.ToLower() == "offline";
   }

    /// <summary>
    /// Contains string detail about what threshold was breached
    /// </summary>
    /// <returns>String describing threshold breach</returns>
   public string ThresholdDetail()
   {
        return "Active but not online";
   }
}
