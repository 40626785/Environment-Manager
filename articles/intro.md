# Introduction to Environment Manager

The Environment Manager is a .NET MAUI application designed for comprehensive environment and sensor management. This article provides an overview of the system architecture and key components.

## Architecture Overview

### Database Contexts

The application uses a multi-context database architecture for better separation of concerns:

1. **MaintenanceDbContext**
   - Handles maintenance records
   - Tracks maintenance schedules and priorities
   - Manages overdue status

2. **LocationDbContext**
   - Manages site locations
   - Stores geographical data
   - Handles timezone information

3. **SensorDbContext**
   - Manages sensor information
   - Tracks sensor status and configurations
   - Handles sensor readings

4. **EnvironmentalParameterDbContext**
   - Manages environmental parameter definitions
   - Stores measurement units and safe levels
   - Tracks parameter categories

### Key Services

#### DatabaseInitializationService

The `DatabaseInitializationService` is responsible for:
- Verifying database connections
- Loading test data in development environments
- Managing database initialization
- Handling connection errors gracefully

```csharp
public interface IDatabaseInitializationService
{
    Task VerifyDatabaseConnectionsAsync();
    Task LoadTestDataIfNeededAsync();
}
```

### ViewModels

The application follows the MVVM pattern with several key ViewModels:

1. **AddSensorViewModel**
   - Handles sensor creation
   - Validates sensor input
   - Manages location selection

2. **EditSensorViewModel**
   - Handles sensor updates
   - Validates modifications
   - Manages sensor state

3. **MaintenanceViewModel**
   - Manages maintenance tasks
   - Handles priority tracking
   - Updates maintenance status

4. **AllMaintenanceViewModel**
   - Displays maintenance overview
   - Sorts by priority
   - Manages maintenance collection

## Development Guidelines

1. **Database Operations**
   - Use appropriate DbContext for each operation
   - Implement proper error handling
   - Follow async/await patterns

2. **Testing**
   - Unit tests for ViewModels
   - Integration tests for database operations
   - Mock dependencies appropriately

3. **Error Handling**
   - Implement IErrorHandling interface
   - Use appropriate error messages
   - Log errors for debugging

## Configuration

The application uses appsettings.json for configuration:
```json
{
    "ConnectionStrings": {
        "DevelopmentConnection": "Server=localhost;Database=environmentdb;User Id=environmentapp;Password=<password>;TrustServerCertificate=True;Encrypt=True;"
    }
}
```
