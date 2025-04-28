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
    BatteryLevelPercentage REAL,
    CONSTRAINT FK_Sensors_Locations FOREIGN KEY (LocationId) REFERENCES Locations(LocationId)
);

-- Create Maintenance table
CREATE TABLE Maintenance (
    id INT IDENTITY(1,1) PRIMARY KEY,
    DueDate DATETIME2 DEFAULT SYSDATETIME(),
    Overdue BIT DEFAULT 0,
    Priority INT NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
);

-- Create SensorStatus table for real-time monitoring
CREATE TABLE SensorStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    SensorId INT NOT NULL,
    ConnectivityStatus NVARCHAR(50) NOT NULL DEFAULT 'Offline',
    StatusTimestamp DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    BatteryLevelPercentage REAL,
    ErrorCount INT DEFAULT 0,
    WarningCount INT DEFAULT 0,
    CONSTRAINT FK_SensorStatus_Sensors FOREIGN KEY (SensorId) REFERENCES Sensors(SensorId)
);

-- Create Roles table to define system roles
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200),
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    LastModifiedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

-- Create User table
CREATE TABLE Users (
    Username VARCHAR(20) PRIMARY KEY,
    Password VARCHAR(20) NOT NULL,
    Role INT NOT NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (Role) REFERENCES Roles(RoleId)
);

CREATE TABLE Readings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Timestamp DATETIME NOT NULL,
    Category NVARCHAR(50) NOT NULL,

    -- Air Quality
    NitrogenDioxide FLOAT NULL,
    SulphurDioxide FLOAT NULL,
    PM25 FLOAT NULL,
    PM10 FLOAT NULL,

    -- Water Quality
    Nitrite FLOAT NULL,
    Nitrate FLOAT NULL,
    Phosphate FLOAT NULL,
    EColi INT NULL,
    Enterococci INT NULL,

    -- Weather
    Temperature FLOAT NULL,
    Humidity FLOAT NULL,
    WindSpeed FLOAT NULL,
    WindDirection FLOAT NULL
);


-- Create UserLogs table for tracking user changes
CREATE TABLE UserLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(255) NOT NULL,
    ActionType NVARCHAR(50) NOT NULL,
    ChangedFields NVARCHAR(MAX) NULL,
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    PerformedBy NVARCHAR(255) NULL,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE()
);