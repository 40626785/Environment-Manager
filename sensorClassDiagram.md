```mermaid
classDiagram
    Sensor "1" <|-- "*" SensorReading
    Sensor "1" <|-- "*" SensorSetting
    Location "1" <|-- "*" Sensor
    EnvironmentalParameter "1" <|-- "*" SensorReading

    class Sensor {
        -int sensorId
        -int locationId
        -string sensorName
        -string model
        -string manufacturer
        -string sensorType
        -DateTime installationDate
        -bool isActive
        -string firmwareVersion
        -string dataSource
        -string sensorUrl
        -string connectivityStatus
        -float? batteryLevelPercentage
        -List<SensorReading> readings
        -List<SensorSetting> settings
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
        +ICollection<SensorReading> Readings
        +ICollection<SensorSetting> Settings
        +Location Location
        +Sensor()
    }

    class SensorReading {
        -int readingId
        -int sensorId
        -int parameterId
        -DateTime timestamp
        -float value
        -string measurementUnit
        -bool isValid
        +int ReadingId
        +int SensorId
        +int ParameterId
        +DateTime Timestamp
        +float Value
        +string MeasurementUnit
        +bool IsValid
        +Sensor Sensor
        +EnvironmentalParameter EnvironmentalParameter
        +SensorReading()
    }

    class SensorSetting {
        -int settingId
        -int sensorId
        -string settingName
        -string settingValue
        -string dataType
        -DateTime lastUpdated
        +int SettingId
        +int SensorId
        +string SettingName
        +string SettingValue
        +string DataType
        +DateTime LastUpdated
        +Sensor Sensor
        +SensorSetting()
    }

    class Location {
        <<external>>
    }

    class EnvironmentalParameter {
        <<external>>
    }````
