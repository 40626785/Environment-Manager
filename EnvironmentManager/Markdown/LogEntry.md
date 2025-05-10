classDiagram
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
        +void OnModelCreating(ModelBuilder)
    }

    class ErrorDbContext {
        +DbSet<ErrorEntry> Errors
        +ErrorDbContext(DbContextOptions)
        +void OnModelCreating(ModelBuilder)
    }

    class IUserDialogService {
        +Task ShowAlert(string, string, string)
        +Task<bool> ShowConfirmation(string, string, string, string)
        +Task NavigateBackAsync()
    }

    class LogViewModel {
        +ObservableCollection<LogEntry> TableData
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +ICommand ToggleFilterVisibilityCommand
        +LogViewModel(IDbContextFactory, IUserDialogService)
        +Task LoadDataAsync()
        +Task ApplyFiltersAsync()
        +Task DeleteFilteredAsync()
        +Task ExportToCsvAsync()
    }

    class ErrorViewModel {
        +ObservableCollection<ErrorEntry> TableData
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +ICommand ToggleFilterVisibilityCommand
        +ErrorViewModel(IDbContextFactory, IUserDialogService)
        +Task LoadDataAsync()
        +Task ApplyFiltersAsync()
        +Task DeleteFilteredAsync()
        +Task ExportToCsvAsync()
    }

    class LogPage {
        +LogPage(LogViewModel)
    }

    class ErrorPage {
        +ErrorPage(ErrorViewModel)
    }

    LogViewModel --> LogDbContext : Uses
    ErrorViewModel --> ErrorDbContext : Uses
    LogDbContext --> LogEntry : Contains
    ErrorDbContext --> ErrorEntry : Contains
    LogPage --> LogViewModel : Binds
    ErrorPage --> ErrorViewModel : Binds
    LogViewModel --> IUserDialogService : Uses
    ErrorViewModel --> IUserDialogService : Uses

