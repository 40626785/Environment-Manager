using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;
/// <summary>
/// Enables Dependency Inversion Principle, allowing ViewModels to depend on Interface rather than a concrete context class
/// 
/// Implementation decided in MauiProgram.cs
/// </summary>
public interface IMaintenanceDataStore
{
    IEnumerable<Maintenance> RetrieveAll();
    Maintenance QueryById(int id);
    void Update(Maintenance maintenance);
    void Delete(Maintenance maintenance);
    void Reload(Maintenance maintenance);
    void Save();
}
