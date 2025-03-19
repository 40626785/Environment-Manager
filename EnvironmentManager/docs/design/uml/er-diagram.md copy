erDiagram
    SCIENTIST ||--o{ SENSOR_ACCOUNT : manages
    SENSOR_ACCOUNT ||--o{ SENSOR : contains
    SENSOR ||--o{ SENSOR_READING : generates
    SENSOR_READING }|--|| AIR_QUALITY_PARAMETER : measures
    SENSOR ||--o{ SENSOR_SETTING : configures
    SENSOR ||--o{ CALIBRATION_RECORD : maintains
    SENSOR ||--o{ MAINTENANCE_RECORD : requires
    SENSOR_ACCOUNT {
        int account_id PK
        int scientist_id FK
        string account_name
        string api_key
        datetime created_date
        boolean is_active
        string description
    }
    SCIENTIST {
        int scientist_id PK
        string username
        string email
        string password_hash
        string specialization
        string organization
        boolean is_active
        datetime created_date
    }
    SENSOR {
        int sensor_id PK
        int account_id FK
        string sensor_name
        string model
        string manufacturer
        string location
        float latitude
        float longitude
        datetime installation_date
        boolean is_active
        datetime last_calibration
        string firmware_version
    }
    SENSOR_READING {
        int reading_id PK
        int sensor_id FK
        int parameter_id FK
        datetime timestamp
        float value
        boolean is_valid
        string notes
    }
    AIR_QUALITY_PARAMETER {
        int parameter_id PK
        string name
        string symbol
        string unit
        string description
        float safe_level
        string reference_url
    }
    SENSOR_SETTING {
        int setting_id PK
        int sensor_id FK
        string setting_name
        string setting_value
        string data_type
        datetime last_updated
    }
    CALIBRATION_RECORD {
        int calibration_id PK
        int sensor_id FK
        datetime calibration_date
        string calibration_method
        string technician
        string notes
    }
    MAINTENANCE_RECORD {
        int maintenance_id PK
        int sensor_id FK
        datetime maintenance_date
        string maintenance_type
        string technician
        string notes
        boolean is_complete
    }
