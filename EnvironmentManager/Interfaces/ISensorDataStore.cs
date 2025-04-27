using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

/// <summary>
/// Enables Dependency Inversion Principle, allowing ViewModels to retrieve sensors without depending on concrete DbContext implementation
/// </summary>
public interface ISensorDataStore
{
    List<Sensor> RetrieveAll();
}
