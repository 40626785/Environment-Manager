classDiagram
    class AirQualityRecord {
        +int Id
        +DateTime? Date
        +TimeSpan? Time
        +double? Nitrogen_dioxide
        +double? Sulphur_dioxide
        +double? PM2_5_particulate_matter
        +double? PM10_particulate_matter
        +int LocationId
    }

    class AirQualityDbContext {
        +DbSet<AirQualityRecord> AirQuality
        +AirQualityDbContext(DbContextOptions)
        +void OnModelCreating(ModelBuilder)
    }

    class AirQualityAdminViewModel {
        +ObservableCollection<AirQualityRecord> TableData
        +string LocationIdText
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand ApplySortCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +AirQualityAdminViewModel(IDbContextFactory, ILoggingService, IUserDialogService)
        +Task LoadDataAsync()
        +Task ApplyFiltersAsync()
        +Task ApplySortAsync()
        +Task DeleteFilteredAsync()
        +Task ExportToCsvAsync()
    }

    class AirQualityPage {
        +AirQualityPage(AirQualityAdminViewModel)
        +void OnAppearing()
    }

    class DatabaseAdminViewModel {
        +ObservableCollection<string> TableOptions
        +string SelectedTable
        +ICommand NavigateToTableCommand
        +DatabaseAdminViewModel(DatabaseAdminDbContext)
        +void LoadTables()
        +Task NavigateToTableAsync()
    }

    class DatabaseAdminPage {
        +DatabaseAdminPage(DatabaseAdminViewModel)
    }

    class IDatabaseAdminDataStore {
        +List<string> GetAllTableNames()
        +Task ClearTableByDateAsync(string, DateTime)
        +Task ClearTableByIdRangeAsync(string, int, int)
        +Task<List<Dictionary<string, object>>> GetFilteredTableDataAsync(string, DateTime?, int?, int?)
    }

    AirQualityAdminViewModel --> AirQualityDbContext : Uses
    AirQualityDbContext --> AirQualityRecord : Contains
    AirQualityPage --> AirQualityAdminViewModel : Binds
    DatabaseAdminViewModel --> IDatabaseAdminDataStore : Uses
    DatabaseAdminPage --> DatabaseAdminViewModel : Binds
    DatabaseAdminPage --> AirQualityPage : Navigates to
