erDiagram
    Archive_Air_Quality {
        int ID PK
        date Date
        time Time
        float Nitrogen_dioxide
        float Sulphur_dioxide
        float PM2_5_particulate_matter
        float PM10_particulate_matter
        int LocationId
    }
    Archive_Water_Quality {
        int ID PK
        date Date
        time Time
        float Nitrate_mg_l_1
        float Nitrite_less_thank_mg_l_1
        float Phosphate_mg_l_1
        float EC_cfu_100ml
        int LocationId
    }
    archive_weather_data {
        int ID PK
        datetime Date_Time
        float temperature_2m
        float relative_humidity_2m
        float wind_speed_10m
        float wind_direction_10m
        int LocationId
    }
    weather_data {
        int ID PK
        datetime Date_Time
        float temperature_2m
        float relative_humidity_2m
        float wind_speed_10m
        float wind_direction_10m
        int LocationId
    }
    Water_Quality {
        int ID PK
        date Date
        time Time
        float Nitrate_mg_l_1
        float Nitrite_less_thank_mg_l_1
        float Phosphate_mg_l_1
        float EC_cfu_100ml
        int LocationId
    }
    Air_Quality {
        int ID PK
        date Date
        time Time
        float Nitrogen_dioxide
        float Sulphur_dioxide
        float PM2_5_particulate_matter
        float PM10_particulate_matter
        int LocationId
    }
    ErrorTable {
        int ErrorID PK
        datetime ErrorDateTime
        nvarchar ErrorMessage
    }
    LogTable {
        int LogID PK
        datetime LogDateTime
        nvarchar LogMessage
    }
    AlertTable {
        int AlertId PK
        int LocationId
        datetime Date_Time
        nvarchar Parameter
        float Value
        float Deviation
        nvarchar Message
        datetime CreatedAt
        bit IsResolved
    }

    %% Triggers and Procedures as Simple Entities
    Trigger_WeatherData {
        string Name "AfterInsert_WeatherData"
    }
    Trigger_WaterQuality {
        string Name "AfterInsert_WaterQuality"
    }
    Trigger_AirQuality {
        string Name "AfterInsert_AirQuality"
    }

    Procedure_CheckAirQualityAnomalies {
        string Name "CheckAirQualityAnomalies"
    }
    Procedure_CheckWaterQualityAnomalies {
        string Name "CheckWaterQualityAnomalies"
    }
    Procedure_CheckWeatherDataAnomalies {
        string Name "CheckWeatherDataAnomalies"
    }

    %% Relationships
    Archive_Air_Quality ||--o{ Air_Quality : contains
    Archive_Water_Quality ||--o{ Water_Quality : contains
    archive_weather_data ||--o{ weather_data : contains
    ErrorTable ||--o{ LogTable : contains
    LogError ||--o{ LogTable : logs
    LogMessage ||--o{ LogTable : logs
    AlertTable ||--o{ Locations : associated with

    %% Trigger Relationships
    Trigger_WeatherData ||--o{ weather_data : triggers
    Trigger_WaterQuality ||--o{ Water_Quality : triggers
    Trigger_AirQuality ||--o{ Air_Quality : triggers

    %% Procedure Relationships
    Procedure_CheckAirQualityAnomalies ||--o{ LogTable : logs
    Procedure_CheckWaterQualityAnomalies ||--o{ LogTable : logs
    Procedure_CheckWeatherDataAnomalies ||--o{ LogTable : logs
