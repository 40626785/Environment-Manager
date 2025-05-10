classDiagram
    class DatabaseAdminPage {
        +DatabaseAdminPage(DatabaseAdminViewModel)
    }

    class IDatabaseAdminDataStore {
        +List<string> GetAllTableNames()
        +Task ClearTableByDateAsync(string, DateTime)
        +Task ClearTableByIdRangeAsync(string, int, int)
        +Task<List<Dictionary<string, object>>> GetFilteredTableDataAsync(string, DateTime?, int?, int?)
    }

    class LogEntry {
        +int LogID
        +DateTime? LogDateTime
        +string LogMessage
    }

    class ErrorEntry {
        +int ErrorID
        +DateTime? ErrorDateTime
        +string ErrorMessage
    }

    class ArchiveAirQuality {
        +int Id
        +DateTime? Date
        +TimeSpan? Time
        +double? Nitrogen_dioxide
        +double? Sulphur_dioxide
        +double? PM2_5_particulate_matter
        +double? PM10_particulate_matter
        +int LocationId
    }

    class LogDbContext {
        +DbSet<LogEntry> Logs
        +LogDbContext(DbContextOptions)
    }

    class ErrorDbContext {
        +DbSet<ErrorEntry> Errors
        +ErrorDbContext(DbContextOptions)
    }

    class ArchiveAirQualityDbContext {
        +DbSet<ArchiveAirQuality> ArchiveAirQuality
        +ArchiveAirQualityDbContext(DbContextOptions)
    }

    class LogViewModel {
        +ObservableCollection<LogEntry> TableData
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +ICommand ToggleFilterVisibilityCommand
        +LogViewModel(IDbContextFactory, IUserDialogService)
    }

    class ErrorViewModel {
        +ObservableCollection<ErrorEntry> TableData
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +ICommand ToggleFilterVisibilityCommand
        +ErrorViewModel(IDbContextFactory, IUserDialogService)
    }

    class ArchiveAirQualityViewModel {
        +ObservableCollection<ArchiveAirQuality> TableData
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +ICommand ToggleFilterVisibilityCommand
        +ArchiveAirQualityViewModel(IDbContextFactory, ILoggingService, IUserDialogService)
    }

    class LogPage {
        +LogPage(LogViewModel)
    }

    class ErrorPage {
        +ErrorPage(ErrorViewModel)
    }

    class ArchiveAirQualityPage {
        +ArchiveAirQualityPage(ArchiveAirQualityViewModel)
    }

    LogViewModel --> LogDbContext : Uses
    ErrorViewModel --> ErrorDbContext : Uses
    ArchiveAirQualityViewModel --> ArchiveAirQualityDbContext : Uses
    LogDbContext --> LogEntry : Contains
    ErrorDbContext --> ErrorEntry : Contains
    ArchiveAirQualityDbContext --> ArchiveAirQuality : Contains
    LogPage --> LogViewModel : Binds
    ErrorPage --> ErrorViewModel : Binds
    ArchiveAirQualityPage --> ArchiveAirQualityViewModel : Binds

    %% Navigation connections
    DatabaseAdminPage --> LogPage : Navigates to
    DatabaseAdminPage --> ErrorPage : Navigates to
    DatabaseAdminPage --> ArchiveAirQualityPage : Navigates to
