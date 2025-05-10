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
    }

    class HistoricalDataPage {
        +HistoricalDataPage(HistoricalDataSelectionViewModel)
    }

    class HistoricalDataViewerPage {
        +string TableName
        +HistoricalDataViewerPage()
    }

    HistoricalDataSelectionViewModel --> HistoricalDataViewerPage
    HistoricalDataViewerViewModel --> HistoricalDataDbContext
    HistoricalDataViewerPage --> HistoricalDataViewerViewModel
    HistoricalDataPage --> HistoricalDataSelectionViewModel
    HistoricalDataViewerViewModel --> HistoricalAirQualityDbContext
    HistoricalAirQualityDbContext --> ArchiveAirQuality
    HistoricalDataDbContext --> ArchiveAirQuality
    HistoricalDataDbContext --> ArchiveWaterQuality
    HistoricalDataDbContext --> ArchiveWeatherData
