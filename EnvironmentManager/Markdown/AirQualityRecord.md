classDiagram

%% === Models ===
class AirQualityRecord {
  +int Id
  +DateTime? Date
  +TimeSpan? Time
  +double? Nitrogen_dioxide
  +double? Sulphur_dioxide
  +double? PM2_5_particulate_matter
  +double? PM10_particulate_matter
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

%% === DbContexts ===
class AirQualityDbContext {
  +DbSet<AirQualityRecord> AirQuality
}

class ArchiveAirQualityDbContext {
  +DbSet<ArchiveAirQuality> ArchiveAirQuality
}

%% === ViewModels ===
class AirQualityAdminViewModel
class ArchiveAirQualityViewModel
class EditArchiveAirQualityViewModel

%% === Services ===
class IUserDialogService
class NavigationDataStore

%% === Relationships ===
AirQualityDbContext --> AirQualityRecord : DbSet
ArchiveAirQualityDbContext --> ArchiveAirQuality : DbSet

AirQualityAdminViewModel --> AirQualityRecord : TableData
AirQualityAdminViewModel --> IUserDialogService

ArchiveAirQualityViewModel --> ArchiveAirQuality : TableData
ArchiveAirQualityViewModel --> ArchiveAirQualityDbContext
ArchiveAirQualityViewModel --> IUserDialogService

EditArchiveAirQualityViewModel --> ArchiveAirQuality : EditableRecord
EditArchiveAirQualityViewModel --> ArchiveAirQualityDbContext
EditArchiveAirQualityViewModel --> IUserDialogService

NavigationDataStore --> ArchiveAirQuality : SelectedArchiveAirQualityRecord
