-- Insert Locations (realistic values from Excel)
INSERT INTO Locations (SiteName, Latitude, Longitude, Elevation, SiteType, Zone, Agglomeration, LocalAuthority, Country, UtcOffsetSeconds, Timezone, TimezoneAbbreviation)
VALUES 
('Edinburgh Nicolson Street', 55.94476, -3.183991, 50.0, 'Urban Traffic', 'Central', 'Edinburgh', 'City of Edinburgh', 'United Kingdom', 0, 'Europe/London', 'GMT'),
('Glencorse B', 55.8611, -3.2539, 180.0, 'River', 'South', 'Midlothian', 'Midlothian Council', 'United Kingdom', 0, 'Europe/London', 'GMT'),
('Holyrood Park', 55.9469, -3.1615, 120.0, 'Park', 'East', 'Edinburgh', 'City of Edinburgh', 'United Kingdom', 0, 'Europe/London', 'GMT');

-- Insert Sensors (linked to Locations)
INSERT INTO Sensors (LocationId, SensorName, Model, Manufacturer, SensorType, FirmwareVersion, DataSource, SensorUrl, ConnectivityStatus, BatteryLevelPercentage)
VALUES
(1, 'Airly-AQ1', 'AQ-2024', 'Airly', 'Air Quality', 'v1.2.0', 'UK-AIR API', 'https://airly.org/device/aq1', 'Online', 95.5),
(2, 'ClearWater-WQ1', 'WQ-Node', 'ClearWater', 'Water Quality', 'v3.0.1', 'RiverNet', 'https://clearwatersensors.com/wq1', 'Online', 88.2),
(3, 'MetNet-WX1', 'WX-Pro', 'MetNet', 'Weather', 'v2.1.4', 'WeatherAPI', 'https://metnet.com/wx1', 'Offline', 67.0);

-- Insert Maintenance tasks
INSERT INTO Maintenance (DueDate, Overdue, Priority, Description)
VALUES 
(DATEADD(DAY, 7, SYSDATETIME()), 0, 1, 'Replace air filter at Edinburgh Nicolson Street station'),
(DATEADD(DAY, -2, SYSDATETIME()), 1, 2, 'Battery maintenance for ClearWater sensor at Glencorse'),
(DATEADD(DAY, 10, SYSDATETIME()), 0, 3, 'Firmware upgrade for MetNet weather node at Holyrood');

-- Insert roles as specified in the requirements
-- Ensure RoleIds match the Roles enum definition (BasicUser=0, Administrator=1, etc.)
INSERT INTO Roles (RoleId, RoleName, Description)
VALUES 
    (0, 'BasicUser', 'Access to basic features'),
    (1, 'Administrator', 'Full system access with user management capabilities'),
    (2, 'EnvironmentalScientist', 'Access to scientific data and analysis tools'),
    (3, 'OperationsManager', 'Access to operational data and management functions');

-- Insert test users with correct RoleIds matching the enum/Roles table
INSERT INTO Users (Username, Password, Role)
VALUES
    ('admin', 'admin123', 1), -- Administrator (RoleId = 1)
    ('manager', 'manager123', 3), -- OperationsManager (RoleId = 3)
    ('scientist', 'scientist123', 2); -- EnvironmentalScientist (RoleId = 2)
    -- Add a BasicUser example if needed
    -- ('basic', 'basic123', 0), -- BasicUser (RoleId = 0)

