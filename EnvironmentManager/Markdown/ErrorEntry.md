classDiagram

%% === Model ===
class LogEntry {
  +int LogID
  +DateTime LogDateTime
  +string LogMessage
}

%% === DbContext ===
class LogDbContext {
  +DbSet<LogEntry> LogTable
}

%% === ViewModel ===
class LogViewModel {
  +ObservableCollection<LogEntry> TableData
  +Task LoadDataAsync()
  +Task ApplyFiltersAsync()
  +Task DeleteFilteredAsync()
  +Task ExportToCsvAsync()
}

%% === Services ===
class IUserDialogService
class ILoggingService

%% === Navigation ===
class NavigationDataStore {
  +LogEntry? SelectedLogEntryRecord
}

%% === Relationships ===
LogDbContext --> LogEntry : DbSet

LogViewModel --> LogEntry
LogViewModel --> LogDbContext
LogViewModel --> IUserDialogService
LogViewModel --> ILoggingService

NavigationDataStore --> LogEntry : SelectedLogEntryRecord
