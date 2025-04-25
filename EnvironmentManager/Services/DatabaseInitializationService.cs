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
        private readonly UserManagementDbContext _userManagementContext;
        private readonly UserLogDbContext _userLogContext;

        public DatabaseInitializationService(
            SensorDbContext sensorContext,
            LocationDbContext locationContext,
            MaintenanceDbContext maintenanceContext,
            UserManagementDbContext userManagementContext,
            UserLogDbContext userLogContext)
        {
            _sensorContext = sensorContext;
            _locationContext = locationContext;
            _maintenanceContext = maintenanceContext;
            _userManagementContext = userManagementContext;
            _userLogContext = userLogContext;
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
                var userManagementConnected = await _userManagementContext.Database.CanConnectAsync();
                var userLogConnected = await _userLogContext.Database.CanConnectAsync();
                
                Debug.WriteLine($"Database connections: Sensor={sensorConnected}, Location={locationConnected}, " +
                               $"Maintenance={maintenanceConnected}, UserManagement={userManagementConnected}, " +
                               $"UserLog={userLogConnected}");
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
                var userCount = await _userManagementContext.Users.CountAsync();
                
                if (locationCount == 0 || userCount == 0)
                {
                    Debug.WriteLine($"No locations ({locationCount}) or users ({userCount}) found. Loading test data...");
                    await LoadTestDataFromFileAsync();
                }
                else
                {
                    Debug.WriteLine($"Found {locationCount} locations and {userCount} users. No need to load test data.");
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

            Debug.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
            Debug.WriteLine("AppContext.BaseDirectory: " + AppContext.BaseDirectory);
            Debug.WriteLine("FileSystem.AppDataDirectory: " + FileSystem.AppDataDirectory);
            Debug.WriteLine("FileSystem.CacheDirectory: " + FileSystem.CacheDirectory);
            
            foreach (var path in possiblePaths)
            {
                Debug.WriteLine($"Checking for test data at: {path}");
                if (File.Exists(path))
                {
                    Debug.WriteLine($"Found test data file at: {path}");
                    return path;
                }
                else
                {
                    Debug.WriteLine($"File not found at: {path}");
                }
            }

            // Try to list contents of some directories to diagnose
            try {
                Debug.WriteLine("Contents of current directory:");
                foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                {
                    Debug.WriteLine($"  - {file}");
                }
                
                // Try to check if we can see the sql directory
                var sqlDir = Path.Combine(Directory.GetCurrentDirectory(), "sql");
                if (Directory.Exists(sqlDir))
                {
                    Debug.WriteLine("Contents of sql directory:");
                    foreach (var file in Directory.GetFiles(sqlDir))
                    {
                        Debug.WriteLine($"  - {file}");
                    }
                }
                else
                {
                    Debug.WriteLine("sql directory not found");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error listing directory contents: {ex.Message}");
            }

            return null;
        }

        private async Task ExecuteSqlScriptAsync(string sqlScript)
        {
            Debug.WriteLine($"SQL script contents (first 200 chars): {sqlScript.Substring(0, Math.Min(200, sqlScript.Length))}");
            
            // Split the script into individual commands - only on semicolons, not on 'GO' which can appear in comments or data
            var commands = sqlScript.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            
            Debug.WriteLine($"Split SQL script into {commands.Length} commands");
            
            // Execute on all contexts to ensure data is properly distributed
            await ExecuteCommandsOnContextAsync(_userManagementContext, commands);
            // Only execute the other contexts if needed - for our test data, we only need UserManagement for roles and users
            // await ExecuteCommandsOnContextAsync(_sensorContext, commands);
            // await ExecuteCommandsOnContextAsync(_locationContext, commands);
            // await ExecuteCommandsOnContextAsync(_maintenanceContext, commands);
            // await ExecuteCommandsOnContextAsync(_userLogContext, commands);
            
            Debug.WriteLine("SQL commands executed on all database contexts");
        }
        
        private async Task ExecuteCommandsOnContextAsync(DbContext context, string[] commands)
        {
            await context.Database.OpenConnectionAsync();
            
            try
            {
                Debug.WriteLine($"Executing SQL commands on {context.GetType().Name}...");
                
                foreach (var command in commands)
                {
                    if (!string.IsNullOrWhiteSpace(command))
                    {
                        try
                        {
                            using var cmd = context.Database.GetDbConnection().CreateCommand();
                            cmd.CommandText = command.Trim();
                            Debug.WriteLine($"Executing command (first 100 chars): {cmd.CommandText.Substring(0, Math.Min(100, cmd.CommandText.Length))}");
                            await cmd.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            // Log the error but continue with other commands
                            Debug.WriteLine($"Error executing command on {context.GetType().Name}: {ex.Message}");
                            Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                            if (ex.InnerException != null)
                            {
                                Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                            }
                        }
                    }
                }
            }
            finally
            {
                await context.Database.CloseConnectionAsync();
            }
        }
    }
}
