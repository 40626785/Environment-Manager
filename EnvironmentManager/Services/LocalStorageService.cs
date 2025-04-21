using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Services;

public class LocalStorageService : ILocalStorageService
{
    public void SetStringValue(string key, string value)
    {
        Preferences.Set(key,value);
    }
    
    public string GetStringValue(string key)
    {
        return Preferences.Get(key,"");
    }
}
