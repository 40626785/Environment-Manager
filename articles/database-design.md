# Database Design

The Environment Manager uses SQL Server with a well-structured schema design. This article details the database tables and their relationships.

## Tables

### Locations
```sql
CREATE TABLE Locations (
    LocationId INT IDENTITY(1,1) PRIMARY KEY,
    SiteName NVARCHAR(100) NOT NULL,
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    Elevation FLOAT NOT NULL,
    SiteType NVARCHAR(50) NOT NULL,
    Zone NVARCHAR(50) NOT NULL,
    Agglomeration NVARCHAR(100) NOT NULL,
    LocalAuthority NVARCHAR(100) NOT NULL,
    Country NVARCHAR(50) NOT NULL,
    UtcOffsetSeconds INT NOT NULL,
    Timezone NVARCHAR(50) NOT NULL,
    TimezoneAbbreviation NVARCHAR(10) NOT NULL
)
```

### Sensors
```sql
CREATE TABLE Sensors (
    SensorId INT IDENTITY(1,1) PRIMARY KEY,
    LocationId INT NOT NULL,
    SensorName NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Manufacturer NVARCHAR(100) NOT NULL,
    SensorType NVARCHAR(50) NOT NULL,
    InstallationDate DATETIME2 NOT NULL,
    IsActive BIT NOT NULL,
    FirmwareVersion NVARCHAR(50),
    DataSource NVARCHAR(200),
    SensorUrl NVARCHAR(255),
    ConnectivityStatus NVARCHAR(50) DEFAULT 'Offline',
    BatteryLevelPercentage REAL
)
```

### EnvironmentalParameters
```sql
CREATE TABLE EnvironmentalParameters (
    ParameterId INT IDENTITY(1,1) PRIMARY KEY,
    Category NVARCHAR(50) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Symbol NVARCHAR(20) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    UnitDescription NVARCHAR(200) NOT NULL,
    MeasurementFrequency NVARCHAR(50) NOT NULL,
    SafeLevel FLOAT NOT NULL,
    ReferenceUrl NVARCHAR(255)
)
```

### SensorReadings
```sql
CREATE TABLE SensorReadings (
    ReadingId INT IDENTITY(1,1) PRIMARY KEY,
    SensorId INT NOT NULL,
    ParameterId INT NOT NULL,
    Timestamp DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Value FLOAT NOT NULL,
    MeasurementUnit NVARCHAR(20) NOT NULL,
    IsValid BIT NOT NULL DEFAULT 1
)
```

### Maintenance
```sql
CREATE TABLE maintenance (
    id INT IDENTITY(1,1) PRIMARY KEY,
    DueDate DATETIME2 DEFAULT SYSDATETIME(),
    Overdue BIT DEFAULT 0,
    Priority INT NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
)
```

## Database Contexts

The application uses multiple DbContext classes to manage different aspects of the system:

1. **MaintenanceDbContext**
   - Manages maintenance records
   - Handles maintenance scheduling
   - Tracks maintenance priorities

2. **LocationDbContext**
   - Manages location data
   - Handles geographical information
   - Stores site details

3. **SensorDbContext**
   - Manages sensor information
   - Tracks sensor status
   - Handles sensor readings

4. **EnvironmentalParameterDbContext**
   - Manages parameter definitions
   - Stores measurement units
   - Tracks safe levels

## Database Initialization

The database initialization process is handled by the `DatabaseInitializationService`, which:

1. Verifies database connections
2. Loads test data in development environments
3. Handles connection errors gracefully
4. Manages database schema updates
