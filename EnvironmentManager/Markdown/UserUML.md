classDiagram

%% === Models ===
class User {
  +string Username
  +string Password
  +int Role
}

class Location {
  +int LocationId
  +string SiteName
  +double Latitude
  +double Longitude
  +double Elevation
  +string SiteType
  +string Zone
  +string Agglomeration
  +string LocalAuthority
  +string Country
  +int UtcOffsetSeconds
  +string Timezone
  +string TimezoneAbbreviation
}

class LogEntry {
  +int LogID
  +DateTime LogDateTime
  +string LogMessage
}

class ErrorEntry {
  +int ErrorID
  +DateTime ErrorDateTime
  +string ErrorMessage
}

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

%% === ViewModels ===
class AdminUserViewModel
class AddUserViewModel
class EditUserViewModel

class AdminLocationViewModel
class EditLocationViewModel

class LogViewModel
class ErrorViewModel

class AirQualityAdminViewModel
class ArchiveAirQualityViewModel
class EditArchiveAirQualityViewModel

%% === Services ===
class IUserDialogService
class ILoggingService
class NavigationDataStore

%% === DbContexts ===
class UserDbContext
class LocationDbContext
class LogDbContext
class ErrorDbContext
class AirQualityDbContext
class ArchiveAirQualityDbContext

%% === Relationships ===
UserDbContext --> User
LocationDbContext --> Location
LogDbContext --> LogEntry
ErrorDbContext --> ErrorEntry
AirQualityDbContext --> AirQualityRecord
ArchiveAirQualityDbContext --> ArchiveAirQuality

NavigationDataStore --> User : SelectedUserRecord
NavigationDataStore --> Location : SelectedLocationRecord
NavigationDataStore --> ArchiveAirQuality : SelectedArchiveAirQualityRecord

AdminUserViewModel --> User
EditUserViewModel --> User
AddUserViewModel --> User

AdminLocationViewModel --> Location
EditLocationViewModel --> Location

LogViewModel --> LogEntry
ErrorViewModel --> ErrorEntry

AirQualityAdminViewModel --> AirQualityRecord
ArchiveAirQualityViewModel --> ArchiveAirQuality
EditArchiveAirQualityViewModel --> ArchiveAirQuality

AdminUserViewModel --> IUserDialogService
AdminUserViewModel --> IDbContextFactory~UserDbContext~

EditUserViewModel --> UserDbContext
AddUserViewModel --> UserDbContext
