using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Data;
/// <summary>
/// Abstracts the DbContext implementation, providing ViewModels access to any required read/write access without depending on a concrete data management implementation.
/// 
/// Implements ISensorDataStore which is passed into constructors via Dependency Injection.
/// </summary>
public class SensorDataStore : DbContext, ISensorDataStore
{
    private SensorDbContext _context;

    public SensorDataStore(SensorDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all sensors in database
    /// </summary>
    /// <returns>List of sensor objects</returns>
    public List<Sensor> RetrieveAll()
    {
        var sensorsList = _context.Sensors
                    .Include(s => s.Location)  // Include location data
                    .AsNoTracking()
                    .OrderBy(s => s.SensorName)
                    .ToList();
        return sensorsList;
    }
}





                