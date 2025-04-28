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

-- Air Quality: Edinburgh Nicolson Street
INSERT INTO Readings (Timestamp, Category, NitrogenDioxide, SulphurDioxide, PM25, PM10)
VALUES 
('2025-02-01T01:00:00', 'Air', 26.3925, 1.59654, 5.094, 8.3),
('2025-02-01T02:00:00', 'Air', 22.5675, 1.33045, 5.094, 7.9);

-- Water Quality: Glencorse B
INSERT INTO Readings (Timestamp, Category, Nitrate, Nitrite, Phosphate)
VALUES 
('2025-02-01T01:00:00', 'Water', 26.33, 1.33, 0.07),
('2025-02-01T02:00:00', 'Water', 23.4, 1.52, 0.06);

-- Weather Measurements
INSERT INTO Readings (Timestamp, Category, Temperature, Humidity, WindSpeed, WindDirection)
VALUES
('2025-02-01T00:00:00', 'Weather', 0.6, 98, 1.18, 78),
('2025-02-01T01:00:00', 'Weather', 2.4, 96, 0.93, 106),
('2025-02-01T02:00:00', 'Weather', 2.5, 97, 1.08, 103);