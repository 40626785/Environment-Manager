-- Create Locations table
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
);

-- Create Sensors table
CREATE TABLE Sensors (
    SensorId INT IDENTITY(1,1) PRIMARY KEY,
    LocationId INT NOT NULL,
    SensorName NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100),
    Manufacturer NVARCHAR(100),
    SensorType NVARCHAR(50),
    InstallationDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1,
    FirmwareVersion NVARCHAR(50),
    DataSource NVARCHAR(200),
    SensorUrl NVARCHAR(255),
    ConnectivityStatus NVARCHAR(50) DEFAULT 'Offline',
    BatteryLevelPercentage REAL
);

-- Create EnvironmentalParameters table
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
);

-- Create SensorReadings table
CREATE TABLE SensorReadings (
    ReadingId INT IDENTITY(1,1) PRIMARY KEY,
    SensorId INT NOT NULL,
    ParameterId INT NOT NULL,
    Timestamp DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Value FLOAT NOT NULL,
    MeasurementUnit NVARCHAR(20) NOT NULL,
    IsValid BIT NOT NULL DEFAULT 1
);

-- Create SensorSettings table
CREATE TABLE SensorSettings (
    SettingId INT IDENTITY(1,1) PRIMARY KEY,
    SensorId INT NOT NULL,
    SettingName NVARCHAR(100) NOT NULL,
    SettingValue NVARCHAR(MAX) NOT NULL,
    DataType NVARCHAR(50) NOT NULL,
    LastUpdated DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE maintenance (
    id INT IDENTITY(1,1) PRIMARY KEY,
    DueDate DATETIME2 DEFAULT SYSDATETIME(),
    Overdue BIT DEFAULT 0,
    Priority INT NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
);
