```mermaid
classDiagram
    %% Model relationships
    Sensor "1" -- "*" SensorReading : has
    Sensor "1" -- "*" SensorSetting : has
    Location "1" -- "*" Sensor : contains
    EnvironmentalParameter "1" -- "*" SensorReading : measured by
    SensorViewModel -- Sensor : manages
    AddSensorViewModel -- Location : uses
    EditSensorViewModel -- Sensor : edits

    class Sensor {
        +int SensorId
        +int LocationId
        +string SensorName
        +string Model
        +string Manufacturer
        +string SensorType
        +DateTime InstallationDate
        +bool IsActive
        +string FirmwareVersion
        +string DataSource
        +string SensorUrl
        +string ConnectivityStatus
        +float? BatteryLevelPercentage
        +Location Location
        +ICollection~SensorReading~ Readings
        +ICollection~SensorSetting~ Settings
    }

    class SensorViewModel {
        -SensorDbContext _context
        -bool _isLoading
        +ObservableCollection~Sensor~ Sensors
        +Sensor? SelectedSensor
        +bool IsEditing
        +string PageTitle
        +LoadSensorsCommand()
        +NavigateToAddCommand()
        +NavigateToEditCommand(Sensor)
        +PrepareNewSensorCommand()
        +SelectSensorForEditCommand(Sensor)
        +SaveSensorCommand()
    }

    class AddSensorViewModel {
        -SensorDbContext _sensorContext
        -LocationDbContext _locationContext
        +ObservableCollection~Location~ Locations
        +Location SelectedLocation
        +string SensorName
        +string Model
        +string Manufacturer
        +string SensorType
        +DateTime InstallationDate
        +bool IsActive
        +string FirmwareVersion
        +string SensorUrl
        +bool IsOnline
        +float? BatteryLevelPercentage
        +string DataSource
        +ValidateDataSource()
        +ValidateBatteryLevel()
        +ValidateSensorName()
    }

    class SensorReading {
        +int ReadingId
        +int SensorId
        +int ParameterId
        +DateTime Timestamp
        +float Value
        +string MeasurementUnit
        +bool IsValid
        +Sensor Sensor
        +EnvironmentalParameter EnvironmentalParameter
    }

    class SensorSetting {
        +int SettingId
        +int SensorId
        +string SettingName
        +string SettingValue
        +string DataType
        +DateTime LastUpdated
        +Sensor Sensor
    }

    class Location {
        <<external>>
    }

    class EnvironmentalParameter {
        <<external>>
    }
````
