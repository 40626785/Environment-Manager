classDiagram

class EditArchiveAirQualityViewModel {
  -ArchiveAirQualityDbContext _context
  -IUserDialogService _dialogService
  +ArchiveAirQuality EditableRecord
  +Command SaveCommand
  +Task SaveAsync()
}

class ArchiveAirQuality {
  +int Id
  +DateTime Date
  +TimeSpan Time
  +double Nitrogen_dioxide
  +double Sulphur_dioxide
  +double PM2_5_particulate_matter
  +double PM10_particulate_matter
}

class ArchiveAirQualityDbContext {
  +DbSet<ArchiveAirQuality> ArchiveAirQuality
}

class IUserDialogService {
  +Task ShowAlert(string title, string message, string cancel)
  +Task NavigateBackAsync()
}

class NavigationDataStore {
  +ArchiveAirQuality? SelectedArchiveAirQualityRecord
}

EditArchiveAirQualityViewModel --> ArchiveAirQualityDbContext
EditArchiveAirQualityViewModel --> ArchiveAirQuality
EditArchiveAirQualityViewModel --> IUserDialogService
EditArchiveAirQualityViewModel --> NavigationDataStore : EditableRecord
