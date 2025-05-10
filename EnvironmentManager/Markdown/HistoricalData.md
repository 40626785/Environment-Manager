classDiagram
    class HistoricalDataSelectionViewModel {
        +ICommand SelectTableCommand
        +HistoricalDataSelectionViewModel()
        +void NavigateToViewer(string tableName)
    }

    class HistoricalDataViewerViewModel {
        +ICommand LoadAirQualityDataCommand
        +ICommand ExportToCsvCommand
        +ICommand ExportWaterToCsvCommand
        +ICommand ExportWeatherToCsvCommand
        +ICommand ApplyAirQualityFilterCommand
        +ICommand ApplyWaterFilterCommand
        +ICommand ApplyWeatherFilterCommand
        +HistoricalDataViewerViewModel(HistoricalDataDbContext, ILoggingService)
        +Task LoadAirQualityDataAsync(bool)
        +Task LoadWaterQualityDataAsync(bool)
        +Task LoadWeatherDataAsync(bool)
        +void ExportToCsv()
        +void ExportWaterToCsv()
        +void ExportWeatherToCsv()
        -void ApplyAirQualityFilterAsync()
        -void ApplyWaterFilterAsync()
        -void ApplyWeatherFilterAsync()
    }

    class HistoricalAirQualityDbContext {
        +DbSet<ArchiveAirQuality> ArchiveAirQuality
        +HistoricalAirQualityDbContext(DbContextOptions)
    }

    class HistoricalDataDbContext {
        +DbSet<ArchiveAirQuality> ArchiveAirQuality
        +DbSet<ArchiveWaterQuality> ArchiveWaterQuality
        +DbSet<ArchiveWeatherData> ArchiveWeatherData
        +HistoricalDataDbContext()
        +HistoricalDataDbContext(DbContextOptions)
        -void OnModelCreating(ModelBuilder)
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

    class ArchiveWaterQuality {
        +int Id
        +DateTime? Date
        +TimeSpan? Time
        +double? Nitrate_mg_l_1
        +double? Nitrite_less_thank_mg_l_1
        +double? Phosphate_mg_l_1
        +double? EC_cfu_100ml
    }

    class ArchiveWeatherData {
        +DateTime Date_Time
        +double Temperature_2m
        +double Relative_humidity_2m
        +double Wind_speed_10m
        +double Wind_direction_10m
    }

    class HistoricalDataPage {
        +HistoricalDataPage(HistoricalDataSelectionViewModel)
    }

    class HistoricalDataViewerPage {
        +string TableName
        +HistoricalDataViewerPage()
    }

    HistoricalDataSelectionViewModel --> HistoricalDataViewerPage : Navigates to
    HistoricalDataViewerViewModel --> HistoricalDataDbContext : Uses
    HistoricalDataViewerPage --> HistoricalDataViewerViewModel : Binds
    HistoricalDataPage --> HistoricalDataSelectionViewModel : Binds
    HistoricalDataViewerViewModel --> HistoricalAirQualityDbContext : Uses
    HistoricalAirQualityDbContext --> ArchiveAirQuality : Contains
    HistoricalDataDbContext --> ArchiveAirQuality : Contains
    HistoricalDataDbContext --> ArchiveWaterQuality : Contains
    HistoricalDataDbContext --> ArchiveWeatherData : Contains
