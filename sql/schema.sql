CREATE TABLE maintenance (
    id INT IDENTITY(1,1) PRIMARY KEY,
    DueDate DATETIME2 DEFAULT SYSDATETIME(),
    Overdue BIT DEFAULT 0,
    Priority INT NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
);

-- SQL Server syntax based on Sensor.cs model
CREATE TABLE Sensors (
    SensorId INT IDENTITY(1,1) PRIMARY KEY,
    -- LocationId INT NULL, -- Assuming Location table exists or will be added later
    -- AccountId INT NULL, -- Assuming Account table exists or will be added later
    SensorName NVARCHAR(100) NOT NULL,
    Model NVARCHAR(100) NULL,
    Manufacturer NVARCHAR(100) NULL,
    SensorType NVARCHAR(50) NULL,
    InstallationDate DATETIME2 NOT NULL,
    IsActive BIT NOT NULL,
    LastCalibration DATETIME2 NULL,
    FirmwareVersion NVARCHAR(50) NULL,
    DataSource NVARCHAR(200) NULL,
    SensorUrl NVARCHAR(255) NULL,
    LastHeartbeat DATETIME2 NULL,
    ConnectivityStatus NVARCHAR(50) NULL,
    BatteryLevelPercentage REAL NULL, -- Using REAL for float
    SignalStrengthDbm INT NULL
    -- Add FOREIGN KEY constraints here if/when Location and Account tables are defined
    -- CONSTRAINT FK_Sensor_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    -- CONSTRAINT FK_Sensor_Account FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);
