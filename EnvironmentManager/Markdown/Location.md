classDiagram

%% === Model ===
class EnvironmentManager.Models.Location {
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

%% === DbContext ===
class EnvironmentManager.Data.LocationDbContext {
  +DbSet<Location> Locations
}

%% === ViewModels ===
class AdminLocationViewModel
class EditLocationViewModel

%% === Services ===
class IUserDialogService
class NavigationDataStore

%% === Relationships ===
EnvironmentManager.Data.LocationDbContext --> EnvironmentManager.Models.Location : DbSet

AdminLocationViewModel --> EnvironmentManager.Models.Location : TableData
AdminLocationViewModel --> IUserDialogService
AdminLocationViewModel --> NavigationDataStore

EditLocationViewModel --> EnvironmentManager.Models.Location : EditableRecord
EditLocationViewModel --> IUserDialogService
EditLocationViewModel --> LocationDbContext

NavigationDataStore --> EnvironmentManager.Models.Location : SelectedLocationRecord
