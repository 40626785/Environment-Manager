namespace EnvironmentManager.Interfaces;
/// <summary>
/// Enables Dependency Inversion Principle, allowing management of local storage without directly depending on concrete implementation
/// 
/// Implementation of Interface decided in MauiProgram.cs
/// </summary>
public interface ILocalStorageService
{
    public void SetStringValue(string key, string value);
    
    public string GetStringValue(string key);
}
