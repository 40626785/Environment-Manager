classDiagram

class ArchiveAirQualityViewModel {
    +ObservableCollection~ArchiveAirQuality~ TableData
    +string SelectedSortOption
    +string SelectedSortDirection
    +string StartIdText
    +string EndIdText
    +DateTime StartDate
    +DateTime EndDate
    +bool IsDateFilterEnabled
    +bool IsFilterVisible
    +string ToggleFilterText
    +ICommand LoadDataCommand
    +ICommand ApplyFiltersCommand
    +ICommand ApplySortCommand
    +ICommand DeleteFilteredCommand
    +ICommand ExportToCsvCommand
    +ICommand ToggleFilterVisibilityCommand
    +ICommand RowTappedCommand
}

class ArchiveAirQuality {
    +int Id
    +DateTime Date
    +TimeSpan Time
    +double? Nitrogen_dioxide
    +double? Sulphur_dioxide
    +double? PM2_5_particulate_matter
    +double? PM10_particulate_matter
}

class ArchiveAirQualityDbContext {
    +DbSet~ArchiveAirQuality~ ArchiveAirQuality
}

class DatabaseLoggingService {
    +Task LogMessageAsync(string message)
    +Task LogErrorAsync(string errorMessage)
}

class NavigationDataStore {
    <<static>>
    +ArchiveAirQuality SelectedRecord
}

ArchiveAirQualityViewModel --> ArchiveAirQualityDbContext
ArchiveAirQualityViewModel --> DatabaseLoggingService
ArchiveAirQualityViewModel --> NavigationDataStore
ArchiveAirQualityViewModel --> ArchiveAirQuality

ArchiveAirQualityDbContext --> ArchiveAirQuality
