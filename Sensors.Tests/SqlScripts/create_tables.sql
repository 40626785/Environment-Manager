-- Drop tables if they exist (in reverse order of dependency)
IF OBJECT_ID('dbo.SensorReadings', 'U') IS NOT NULL
    DROP TABLE dbo.SensorReadings;

IF OBJECT_ID('dbo.Sensors', 'U') IS NOT NULL
    DROP TABLE dbo.Sensors;

-- Create Sensors table
CREATE TABLE dbo.Sensors (
    SensorId INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(255) NOT NULL,
    Type VARCHAR(100),
    Status VARCHAR(50),
    LastUpdated DATETIME2 DEFAULT GETUTCDATE()
);

-- Create SensorReadings table
CREATE TABLE dbo.SensorReadings (
    ReadingId INT PRIMARY KEY IDENTITY(1,1),
    SensorId INT NOT NULL,
    Value FLOAT,
    Timestamp DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_SensorReadings_Sensors FOREIGN KEY (SensorId)
        REFERENCES dbo.Sensors(SensorId)
        ON DELETE CASCADE -- Optional: Cascade delete if a sensor is deleted
); 