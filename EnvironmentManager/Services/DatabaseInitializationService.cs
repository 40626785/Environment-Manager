using System.Diagnostics;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.Services
{
    public class DatabaseInitializationService : IDatabaseInitializationService
    {
        private readonly SensorDbContext _sensorContext;
        private readonly LocationDbContext _locationContext;
        private readonly MaintenanceDbContext _maintenanceContext;
        private readonly EnvironmentalParameterDbContext _environmentalParameterContext;

        public DatabaseInitializationService(
            SensorDbContext sensorContext,
            LocationDbContext locationContext,
            MaintenanceDbContext maintenanceContext,
            EnvironmentalParameterDbContext environmentalParameterContext)
        {
            _sensorContext = sensorContext;
            _locationContext = locationContext;
            _maintenanceContext = maintenanceContext;
            _environmentalParameterContext = environmentalParameterContext;
        }

        /// <summary>
        /// Verifies database connections without performing extensive testing
        /// </summary>
        public async Task VerifyDatabaseConnectionsAsync()
        {
            try
            {
                Debug.WriteLine("Verifying database connections...");
                
                // Simple connection checks - no schema testing
                var sensorConnected = await _sensorContext.Database.CanConnectAsync();
                var locationConnected = await _locationContext.Database.CanConnectAsync();
                var maintenanceConnected = await _maintenanceContext.Database.CanConnectAsync();
                var environmentalConnected = await _environmentalParameterContext.Database.CanConnectAsync();
                
                Debug.WriteLine($"Database connections: Sensor={sensorConnected}, Location={locationConnected}, " +
                               $"Maintenance={maintenanceConnected}, Environmental={environmentalConnected}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error verifying database connections: {ex.Message}");
                // Just log the error, don't throw - application should still start
            }
        }

        /// <summary>
        /// Loads test data if needed - should only be called in development environments
        /// </summary>
        public async Task LoadTestDataIfNeededAsync()
        {
            try
            {
                // Check if locations exist
                var locationCount = await _locationContext.Locations.CountAsync();
                
                if (locationCount == 0)
                {
                    Debug.WriteLine("No locations found. Loading test data...");
                    await LoadTestDataFromFileAsync();
                }
                else
                {
                    Debug.WriteLine($"Found {locationCount} locations. No need to load test data.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking/loading test data: {ex.Message}");
                // Just log the error, don't throw - application should still start
            }
        }

        private async Task LoadTestDataFromFileAsync()
        {
            try
            {
                var testDataPath = await FindTestDataFileAsync();
                
                if (string.IsNullOrEmpty(testDataPath))
                {
                    Debug.WriteLine("Could not find test_data.sql file in any of the expected locations");
                    return;
                }
                
                Debug.WriteLine($"Found test data file at: {testDataPath}");
                var testDataContent = await File.ReadAllTextAsync(testDataPath);
                
                // Execute test data SQL script
                await ExecuteSqlScriptAsync(testDataContent);
                
                Debug.WriteLine("Test data loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading test data: {ex.Message}");
            }
        }

        private async Task<string> FindTestDataFileAsync()
        {
            var possiblePaths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "sql", "test_data.sql"),
                Path.Combine(FileSystem.AppDataDirectory, "sql", "test_data.sql"),
                Path.Combine(FileSystem.CacheDirectory, "sql", "test_data.sql"),
                "sql/test_data.sql",
                "../sql/test_data.sql"
            };

            foreach (var path in possiblePaths)
            {
                Debug.WriteLine($"Checking for test data at: {path}");
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        private async Task ExecuteSqlScriptAsync(string sqlScript)
        {
            // Open connection
            await _sensorContext.Database.OpenConnectionAsync();
            
            try
            {
                // Split and execute each command separately
                var commands = sqlScript.Split(new[] { "GO", ";" }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var command in commands)
                {
                    if (!string.IsNullOrWhiteSpace(command))
                    {
                        using var cmd = _sensorContext.Database.GetDbConnection().CreateCommand();
                        cmd.CommandText = command.Trim();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            finally
            {
                // Always close connection
                await _sensorContext.Database.CloseConnectionAsync();
            }
        }
    }
}
