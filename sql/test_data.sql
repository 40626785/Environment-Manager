-- Insert test locations for environmental monitoring
INSERT INTO Locations (SiteName, Latitude, Longitude, Elevation, SiteType, Zone, Agglomeration, LocalAuthority, Country, UtcOffsetSeconds, Timezone, TimezoneAbbreviation)
VALUES 
    -- Air Quality Monitoring Stations
    ('City Center AQ Station', 51.5074, -0.1278, 25.0, 'Urban Background', 'Central', 'Greater London', 'City of London', 'United Kingdom', 0, 'Europe/London', 'GMT'),
    ('Industrial Zone Monitor', 51.4839, -0.1257, 15.0, 'Industrial', 'South', 'Greater London', 'Southwark', 'United Kingdom', 0, 'Europe/London', 'GMT'),
    ('Residential Area Station', 51.5204, -0.1093, 30.0, 'Suburban', 'North', 'Greater London', 'Islington', 'United Kingdom', 0, 'Europe/London', 'GMT'),

    -- Water Quality Monitoring Points
    ('Thames River Station 1', 51.5080, -0.1281, 1.0, 'River', 'Central', 'Greater London', 'Westminster', 'United Kingdom', 0, 'Europe/London', 'GMT'),
    ('Reservoir Monitoring Point', 51.5142, -0.1494, 5.0, 'Reservoir', 'West', 'Greater London', 'Camden', 'United Kingdom', 0, 'Europe/London', 'GMT'),
    ('Treatment Plant Output', 51.4892, -0.1334, 2.0, 'Treatment Plant', 'South', 'Greater London', 'Lambeth', 'United Kingdom', 0, 'Europe/London', 'GMT'),

    -- Weather Stations
    ('City Weather Station', 51.5033, -0.1195, 45.0, 'Urban Weather', 'Central', 'Greater London', 'City of London', 'United Kingdom', 0, 'Europe/London', 'GMT'),
    ('Airport Weather Monitor', 51.4700, -0.4543, 25.0, 'Airport', 'West', 'Greater London', 'Hounslow', 'United Kingdom', 0, 'Europe/London', 'GMT'),
    ('Park Meteorological Station', 51.5073, -0.1657, 35.0, 'Park', 'West', 'Greater London', 'Westminster', 'United Kingdom', 0, 'Europe/London', 'GMT');

