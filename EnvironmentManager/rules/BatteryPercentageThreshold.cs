namespace EnvironmentManager.Rules;

using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;

/// <summary>
/// Rule to verify that sensor battery level does not drop below defined threshold
/// </summary>
public class BatteryPercentageThreshold : IThresholdRules<Sensor>
{   
   private int _threshold;

   public BatteryPercentageThreshold(int threshold)
   {
        _threshold = threshold;
   }

   /// <summary>
   /// Checks if provided sensor breaches rule.
   /// </summary>
   /// <param name="sensor">Object to check battery percentage of</param>
   /// <returns>If battery percentage drops below threshold</returns>
   public bool IsBreachedBy(Sensor sensor)
   {
        return sensor.BatteryLevelPercentage < 10;
   }

    /// <summary>
    /// Contains string detail about what threshold was breached
    /// </summary>
    /// <returns>String describing threshold breach</returns>
   public string ThresholdDetail()
   {
        return $"Battery percentage below {_threshold}%";
   }
}
