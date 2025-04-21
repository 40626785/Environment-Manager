```mermaid
classDiagram
    Sensor "1" -- "0..*" SensorStatus : generates
    Sensor "1" -- "0..*" Maintenance : requires
    SensorDbContext -- SensorStatus : manages
    SensorDbContext -- Sensor : references
    SensorMonitoringViewModel -- SensorStatus : monitors
    SensorMonitoringViewModel -- Sensor : tracks
    MaintenanceViewModel -- Maintenance : manages
    AllMaintenanceViewModel -- Maintenance : lists
    
    class Sensor {
        +int SensorId
        +string SensorName
        +string Model
        +string ConnectivityStatus
        +float? BatteryLevelPercentage
        +bool IsActive
        +string SensorType
        +ICollection~SensorStatus~ StatusHistory
        +ICollection~Maintenance~ MaintenanceRecords
    }
    
    class SensorStatus {
        +int StatusId
        +int SensorId
        +Sensor Sensor
        +string ConnectivityStatus
        +DateTime StatusTimestamp
        +float? BatteryLevelPercentage
        +int? ErrorCount
        +int? WarningCount
        +SensorStatus()
    }
    
    class Maintenance {
        +int MaintenanceId
        +int SensorId
        +Sensor Sensor
        +string Description
        +DateTime ScheduledDate
        +DateTime? CompletedDate
        +string Priority
        +string Status
        +string TechnicianName
        +string MaintenanceType
        +Maintenance()
    }
    
    class SensorDbContext {
        +DbSet~Sensor~ Sensors
        +DbSet~SensorStatus~ SensorStatuses
        +DbSet~Maintenance~ Maintenances
        +SensorDbContext()
        +SensorDbContext(DbContextOptions~SensorDbContext~ options)
        #void OnModelCreating(ModelBuilder modelBuilder)
    }
    
    class SensorMonitoringViewModel {
        -SensorDbContext _context
        -bool _isLoading
        -CancellationTokenSource _refreshCancellationTokenSource
        -const int RefreshIntervalSeconds
        +ObservableCollection~SensorStatus~ SensorStatuses
        +ObservableCollection~Sensor~ Sensors
        +SensorStatus? SelectedSensorStatus
        +string FilterBy
        +bool IsRefreshing
        +DateTime LastRefreshTime
        +int TotalSensors
        +int OnlineSensors
        +int OfflineSensors
        +int DegradedSensors
        +int MaintenanceSensors
        +bool AutoRefreshEnabled
        +SensorMonitoringViewModel()
        +SensorMonitoringViewModel(SensorDbContext context)
        +Task LoadSensorStatusesAsync()
        +void StartAutoRefresh()
        +void StopAutoRefresh()
        -Task AutoRefreshAsync(CancellationToken token)
        +Task ViewSensorDetailsAsync(SensorStatus status)
        +Task RefreshNowAsync()
        +void ToggleAutoRefresh()
    }
    
    class MaintenanceViewModel {
        -int _sensorId
        -SensorDbContext _sensorContext
        +ObservableCollection~Maintenance~ MaintenanceRecords
        +string Description
        +DateTime ScheduledDate
        +DateTime? CompletedDate
        +string Priority
        +string Status
        +string TechnicianName
        +string MaintenanceType
        +string SensorName
        +MaintenanceViewModel(SensorDbContext context)
        +Task LoadMaintenanceRecordsAsync(int sensorId)
        +Task SaveMaintenanceAsync()
        +Task DeleteMaintenanceAsync(Maintenance record)
        +Task NavigateBackAsync()
    }
    
    class AllMaintenanceViewModel {
        -SensorDbContext _context
        +ObservableCollection~Maintenance~ AllMaintenanceRecords
        +ObservableCollection~string~ Priorities
        +ObservableCollection~string~ Statuses
        +string SelectedPriority
        +string SelectedStatus
        +DateTime? StartDate
        +DateTime? EndDate
        +AllMaintenanceViewModel(SensorDbContext context)
        +Task LoadAllMaintenanceAsync()
        +Task FilterMaintenanceAsync()
        +Task EditMaintenanceAsync(MaintenanceViewModel model)
        +Task NavigateToSensorAsync(int sensorId)
    }
``` 