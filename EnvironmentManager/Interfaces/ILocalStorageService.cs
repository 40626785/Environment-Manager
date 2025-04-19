namespace EnvironmentManager.Interfaces;

public interface ILocalStorageService
{
    public void SetStringValue(string key, string value);
    
    public string GetStringValue(string key);
}
