classDiagram

%% User Management Section
class User {
    +string Username
    +string Password
    +int Role
}

class UserDbContext {
    +DbSet<User> Users
    +UserDbContext(DbContextOptions)
}

class AdminUserViewModel {
    +ObservableCollection<User> TableData
    +ICommand LoadDataCommand
    +ICommand ApplyFiltersCommand
    +ICommand DeleteFilteredCommand
    +ICommand ExportToCsvCommand
    +ICommand ToggleFilterVisibilityCommand
    +AdminUserViewModel(IDbContextFactory, IUserDialogService)
}

class AddUserViewModel {
    +User NewUser
    +ICommand SaveCommand
    +AddUserViewModel(UserDbContext, IUserDialogService)
}

class AdminUserPage {
    +AdminUserPage(AdminUserViewModel)
}

class AddUserPage {
    +AddUserPage(AddUserViewModel)
}

AdminUserViewModel --> UserDbContext : Uses
AddUserViewModel --> UserDbContext : Uses
AdminUserPage --> AdminUserViewModel : Binds
AddUserPage --> AddUserViewModel : Binds

%% Log and Error Handling Section
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

class LogViewModel {
    +ObservableCollection<LogEntry> TableData
    +ICommand LoadDataCommand
    +LogViewModel(IDbContextFactory, IUserDialogService)
}

class ErrorViewModel {
    +ObservableCollection<ErrorEntry> TableData
    +ICommand LoadDataCommand
    +ErrorViewModel(IDbContextFactory, IUserDialogService)
}

class LogPage {
    +LogPage(LogViewModel)
}

class ErrorPage {
    +ErrorPage(ErrorViewModel)
}

LogViewModel --> LogEntry : Manages
ErrorViewModel --> ErrorEntry : Manages
LogPage --> LogViewModel : Binds
ErrorPage --> ErrorViewModel : Binds

%% Air Quality Section
class AirQualityRecord {
    +int Id
    +DateTime? Date
    +TimeSpan? Time
    +double? Nitrogen_dioxide
    +double? Sulphur_dioxide
}

class AirQualityAdminViewModel {
    +ObservableCollection<AirQualityRecord> TableData
    +ICommand LoadDataCommand
    +AirQualityAdminViewModel(IDbContextFactory, ILoggingService, IUserDialogService)
}

class EditAirQualityViewModel {
    +AirQualityRecord EditableRecord
    +ICommand SaveCommand
    +EditAirQualityViewModel(AirQualityDbContext, IUserDialogService)
}

class AirQualityPage {
    +AirQualityPage(AirQualityAdminViewModel)
}

class EditAirQualityPage {
    +EditAirQualityPage(EditAirQualityViewModel)
}

AirQualityAdminViewModel --> AirQualityRecord : Manages
EditAirQualityViewModel --> AirQualityRecord : Edits
AirQualityPage --> AirQualityAdminViewModel : Binds
EditAirQualityPage --> EditAirQualityViewModel : Binds

%% Database Admin Section
class DatabaseAdminViewModel {
    +ObservableCollection<string> TableOptions
    +ICommand NavigateToTableCommand
    +DatabaseAdminViewModel(IDatabaseAdminDataStore)
}

class DatabaseAdminPage {
    +DatabaseAdminPage(DatabaseAdminViewModel)
}

DatabaseAdminPage --> DatabaseAdminViewModel : Binds
DatabaseAdminViewModel --> IDatabaseAdminDataStore : Uses

%% Navigation
AdminUserPage --> AddUserPage : Navigates to
AirQualityPage --> EditAirQualityPage : Navigates to
DatabaseAdminPage --> LogPage : Navigates to
DatabaseAdminPage --> ErrorPage : Navigates to
DatabaseAdminPage --> AdminUserPage : Navigates to
DatabaseAdminPage --> AirQualityPage : Navigates to
