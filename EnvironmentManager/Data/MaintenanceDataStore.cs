using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Data;
public class MaintenanceDataStore : DbContext, IMaintenanceDataStore
{
    private MaintenanceDbContext _context;

    public MaintenanceDataStore(MaintenanceDbContext context)
    {
        _context = context;
    }

    public void Delete(Maintenance maintenance)
    {
        _context.Remove(maintenance);
    }

    public Maintenance QueryById(int id)
    {
        return _context.Maintenance.Single(n => n.Id == id);
    }

    public void Reload(Maintenance maintenance)
    {
        _context.Entry(maintenance).Reload();
    }

    public IEnumerable<Maintenance> RetrieveAll() 
    {
        return _context.Maintenance.ToList();
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public void Update(Maintenance maintenance)
    {
        _context.Maintenance.Update(maintenance);
    }
}
