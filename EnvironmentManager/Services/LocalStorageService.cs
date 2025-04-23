using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Services;

/// <summary>
/// Sets values in Preferences (local storage)
/// 
/// Implements ILocalStorageService to enable Dependency Injection 
/// </summary>
public class LocalStorageService : ILocalStorageService
{
    /// <summary>
    /// Stores string value in Preferences based on provided key and string value
    /// </summary>
    /// <param name="key">Name of value, will be used to retrieve value from Preferences</param>
    /// <param name="value">Value to be set for corresponding key</param>
    public void SetStringValue(string key, string value)
    {
        Preferences.Set(key,value);
    }

    /// <summary>
    /// Retrieve string value from Preferences
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetStringValue(string key)
    {
        return Preferences.Get(key,"");
    }
}
