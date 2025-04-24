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

-- Insert roles as specified in the requirements
INSERT INTO Roles (RoleName, Description)
VALUES 
    ('Administrator', 'Full system access with user management capabilities'),
    ('EnvironmentalScientist', 'Access to scientific data and analysis tools'),
    ('OperationsManager', 'Access to operational data and management functions');

-- Insert permissions for different application functions
INSERT INTO Permissions (PermissionName, Description, Resource, Action)
VALUES
    -- User management permissions (Admin only)
    ('manage_users', 'Add, edit or remove user accounts', 'users', 'manage'),
    ('view_users', 'View user accounts', 'users', 'view'),
    ('manage_roles', 'Modify role permissions', 'roles', 'manage'),
    
    -- Sensor management permissions
    ('view_sensors', 'View sensor data', 'sensors', 'view'),
    ('manage_sensors', 'Configure sensors', 'sensors', 'manage'),
    
    -- Location management permissions
    ('view_locations', 'View monitoring locations', 'locations', 'view'),
    ('manage_locations', 'Add or edit monitoring locations', 'locations', 'manage'),
    
    -- Maintenance permissions
    ('view_maintenance', 'View maintenance schedule', 'maintenance', 'view'),
    ('manage_maintenance', 'Schedule and update maintenance tasks', 'maintenance', 'manage'),
    
    -- Report permissions
    ('view_reports', 'View environmental reports', 'reports', 'view'),
    ('create_reports', 'Create new environmental reports', 'reports', 'create');

-- Assign permissions to roles
-- Administrator role (full access)
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 
    (SELECT RoleId FROM Roles WHERE RoleName = 'Administrator'),
    PermissionId
FROM Permissions;

-- Operations Manager role
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 
    (SELECT RoleId FROM Roles WHERE RoleName = 'OperationsManager'),
    PermissionId
FROM Permissions
WHERE PermissionName IN (
    'view_users',
    'view_sensors',
    'manage_sensors',
    'view_locations',
    'manage_locations',
    'view_maintenance',
    'manage_maintenance',
    'view_reports',
    'create_reports'
);

-- Environmental Scientist role
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 
    (SELECT RoleId FROM Roles WHERE RoleName = 'EnvironmentalScientist'),
    PermissionId
FROM Permissions
WHERE PermissionName IN (
    'view_sensors',
    'view_locations',
    'view_maintenance',
    'view_reports',
    'create_reports'
);

-- Insert test users
INSERT INTO Users (Username, Password, Role)
VALUES
    ('admin', 'admin123', (SELECT RoleId FROM Roles WHERE RoleName = 'Administrator')),
    ('manager', 'manager123', (SELECT RoleId FROM Roles WHERE RoleName = 'OperationsManager')),
    ('scientist', 'scientist123', (SELECT RoleId FROM Roles WHERE RoleName = 'EnvironmentalScientist'));

