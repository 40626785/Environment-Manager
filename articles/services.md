# Services

The Environment Manager implements various services to handle different aspects of the application. This article details the key services and their responsibilities.

## DatabaseInitializationService

The `DatabaseInitializationService` is responsible for managing database connections and initialization.

### Interface
```csharp
public interface IDatabaseInitializationService
{
    Task VerifyDatabaseConnectionsAsync();
    Task LoadTestDataIfNeededAsync();
}
```

### Implementation Details

#### Constructor
```csharp
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
```

#### Database Connection Verification
```csharp
public async Task VerifyDatabaseConnectionsAsync()
{
    try
    {
        // Simple connection checks for each context
        var sensorConnected = await _sensorContext.Database.CanConnectAsync();
        var locationConnected = await _locationContext.Database.CanConnectAsync();
        var maintenanceConnected = await _maintenanceContext.Database.CanConnectAsync();
        var environmentalConnected = await _environmentalParameterContext.Database.CanConnectAsync();
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error verifying database connections: {ex.Message}");
    }
}
```

#### Test Data Loading
```csharp
public async Task LoadTestDataIfNeededAsync()
{
    try
    {
        var locationCount = await _locationContext.Locations.CountAsync();
        
        if (locationCount == 0)
        {
            await LoadTestDataFromFileAsync();
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error checking/loading test data: {ex.Message}");
    }
}
```

### Key Features

1. **Connection Management**
   - Verifies connections for all database contexts
   - Handles connection errors gracefully
   - Provides debugging information

2. **Test Data Handling**
   - Checks if test data is needed
   - Loads test data from SQL files
   - Supports multiple file locations
   - Only loads test data in development

3. **Error Handling**
   - Graceful error handling
   - Detailed error logging
   - Non-blocking error recovery

## Service Registration

Services are registered in the `MauiProgram.cs` file:

```csharp
private static void RegisterServices(MauiAppBuilder builder)
{
    // Register DatabaseInitializationService
    builder.Services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();
}
```

## Usage in Application

The services are used throughout the application, primarily in:

1. **Application Startup**
   ```csharp
   public App(IDatabaseInitializationService dbInitService)
   {
       InitializeComponent();
       _dbInitService = dbInitService;
       
       // Initialize database asynchronously
       InitializeDatabaseAsync();
   }
   ```

2. **Database Initialization**
   ```csharp
   private async void InitializeDatabaseAsync()
   {
       try
       {
           await _dbInitService.VerifyDatabaseConnectionsAsync();
           
           #if DEBUG
           await _dbInitService.LoadTestDataIfNeededAsync();
           #endif
       }
       catch (Exception ex)
       {
           Debug.WriteLine($"Error during database initialization: {ex.Message}");
       }
   }
   ```
