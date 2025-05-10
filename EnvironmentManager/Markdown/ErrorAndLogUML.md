classDiagram
    class DatabaseAdminPage {
        +DatabaseAdminPage(DatabaseAdminViewModel)
    }

    class IDatabaseAdminDataStore {
        +List<string> GetAllTableNames()
        +Task ClearTableByDateAsync(string, DateTime)
        +Task ClearTableByIdRangeAsync(string, int, int)
        +Task<List<Dictionary<string, object>>> GetFilteredTableDataAsync(string, DateTime?, int?, int?)
    }

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

    class LogDbContext {
        +DbSet<LogEntry> Logs
        +LogDbContext(DbContextOptions)
    }

    class ErrorDbContext {
        +DbSet<ErrorEntry> Errors
        +ErrorDbContext(DbContextOptions)
    }

    class LogViewModel {
        +ObservableCollection<LogEntry> TableData
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +ICommand ToggleFilterVisibilityCommand
        +LogViewModel(IDbContextFactory, IUserDialogService)
    }

    class ErrorViewModel {
        +ObservableCollection<ErrorEntry> TableData
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +ICommand ToggleFilterVisibilityCommand
        +ErrorViewModel(IDbContextFactory, IUserDialogService)
    }

    class ErrorTableAdminViewModel {
        +ObservableCollection<string> Records
        +int StartId
        +int EndId
        +IRelayCommand ClearCommand
        +ErrorTableAdminViewModel(IDatabaseAdminDataStore)
        +void LoadData()
        +Task ClearAsync()
    }

    class LogPage {
        +LogPage(LogViewModel)
    }

    class ErrorPage {
        +ErrorPage(ErrorViewModel)
    }

    class ErrorTableAdminPage {
        +ErrorTableAdminPage(ErrorTableAdminViewModel)
    }

    LogViewModel --> LogDbContext : Uses
    ErrorViewModel --> ErrorDbContext : Uses
    ErrorTableAdminViewModel --> IDatabaseAdminDataStore : Uses
    LogDbContext --> LogEntry : Contains
    ErrorDbContext --> ErrorEntry : Contains
    LogPage --> LogViewModel : Binds
    ErrorPage --> ErrorViewModel : Binds
    ErrorTableAdminPage --> ErrorTableAdminViewModel : Binds

    %% Navigation connections
    DatabaseAdminPage --> LogPage : Navigates to
    DatabaseAdminPage --> ErrorPage : Navigates to
    DatabaseAdminPage --> ErrorTableAdminPage : Navigates to
