using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Data;

/// <summary>
/// Abstracts the DbContext implementation, providing ViewModels access to any required read/write access without depending on a concrete data management implementation.
/// 
/// Implements IMaintenance DataStore which is passed into constructors via Dependency Injection.
/// </summary>
public class MaintenanceDataStore : DbContext, IMaintenanceDataStore
{
    private MaintenanceDbContext _context;

    public MaintenanceDataStore(MaintenanceDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Removes Maintenance ticket from Database
    /// </summary>
    /// <param name="maintenance">Maintenance object to be removed</param>
    public void Delete(Maintenance maintenance)
    {
        _context.Remove(maintenance);
    }

    /// <summary>
    /// Retrieve single Maintenance ticket based on Id
    /// </summary>
    /// <param name="id">ID of desired ticket</param>
    /// <returns>Returned Maintenance object</returns>
    public Maintenance QueryById(int id)
    {
        return _context.Maintenance.Single(n => n.Id == id);
    }

    /// <summary>
    /// Reloads an entry in the DbContext to keep app and database in sync.
    /// </summary>
    /// <param name="maintenance">Specific Maintenance entry to refresh</param>
    public void Reload(Maintenance maintenance)
    {
        _context.Entry(maintenance).Reload();
    }

    /// <summary>
    /// Retrieves all Maintenance tickets stored in DbContext
    /// </summary>
    /// <returns>Enumerable List containing all stored Maintenance objects</returns>
    public IEnumerable<Maintenance> RetrieveAll() 
    {
        return _context.Maintenance.ToList();
    }

    /// <summary>
    /// Saves changes to all Maintenance objects managed in DbContext
    /// </summary>
    public void Save()
    {
        _context.SaveChanges();
    }

    /// <summary>
    /// Updates entry based on updated property values in provided object.
    /// </summary>
    /// <param name="maintenance">Maintenance entry to update</param>

    public void Update(Maintenance maintenance)
    {
        _context.Maintenance.Update(maintenance);
    }
}
