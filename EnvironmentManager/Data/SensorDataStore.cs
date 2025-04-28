using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Data;

/// <summary>
/// Provides data access operations for Sensor entities, abstracting the underlying DbContext.
/// Implements the ISensorDataStore interface for dependency injection.
/// </summary>
public class SensorDataStore : ISensorDataStore // Note: Typically, a DataStore wouldn't inherit DbContext itself, but rather use an injected one.
{
    private readonly SensorDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SensorDataStore"/> class.
    /// </summary>
    /// <param name="context">The database context for sensors.</param>
    public SensorDataStore(SensorDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all sensors from the database, including their associated Location data.
    /// Sensors are ordered by name.
    /// </summary>
    /// <returns>A list of all Sensor objects with Location included.</returns>
    public List<Sensor> RetrieveAll()
    {
        // Retrieve sensors, including related Location data for context.
        // AsNoTracking() is used for performance improvement as this is a read-only operation.
        var sensorsList = _context.Sensors
                    .Include(s => s.Location)  // Eagerly load the related Location entity
                    .AsNoTracking()           // Detach entities from context tracking for performance
                    .OrderBy(s => s.SensorName)
                    .ToList();
        return sensorsList;
    }

    // Add other data access methods as needed (e.g., GetById, Add, Update, Delete)
    // Example:
    /*
    public async Task<Sensor?> GetSensorByIdAsync(int sensorId)
    {
        return await _context.Sensors
                             .Include(s => s.Location)
                             .AsNoTracking()
                             .FirstOrDefaultAsync(s => s.SensorId == sensorId);
    }
    */
}    