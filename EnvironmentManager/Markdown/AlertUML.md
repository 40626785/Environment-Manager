classDiagram
    class Alert {
        +int AlertId
        +int LocationId
        +DateTime Date_Time
        +string Parameter
        +double? Value
        +double? Deviation
        +string Message
        +DateTime CreatedAt
        +bool IsResolved
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

    class IUserDialogService {
        +Task ShowAlert(string, string, string)
        +Task<bool> ShowConfirmation(string, string, string, string)
        +Task NavigateBackAsync()
    }

    class BaseEntry {
        <<abstract>>
        +int ID
        +DateTime? DateTime
        +string Message
    }

    class BaseViewModel {
        <<abstract>>
        +ObservableCollection<BaseEntry> TableData
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +ICommand ToggleFilterVisibilityCommand
        +Task LoadDataAsync()
        +Task ApplyFiltersAsync()
        +Task DeleteFilteredAsync()
        +Task ExportToCsvAsync()
    }

    class LogViewModel {
        +ObservableCollection<LogEntry> TableData
        +LogViewModel(IDbContextFactory, IUserDialogService)
    }

    class ErrorViewModel {
        +ObservableCollection<ErrorEntry> TableData
        +ErrorViewModel(IDbContextFactory, IUserDialogService)
    }

    class AlertViewModel {
        +ObservableCollection<Alert> ActiveAlerts
        +AlertViewModel(IDbContextFactory<AlertDbContext>)
        +Task LoadActiveAlerts()
        +Task MarkAsResolved(int)
        +Task ViewAllResolvedAlerts()
    }

    class ResolvedAlertsViewModel {
        +ObservableCollection<Alert> ResolvedAlerts
        +ResolvedAlertsViewModel(IDbContextFactory<AlertDbContext>)
        +Task LoadResolvedAlerts()
        +Task DeleteResolvedAlert(int)
    }

    class AlertDbContext {
        +DbSet<Alert> AlertTable
        +AlertDbContext(DbContextOptions)
    }

    class MaintenancePage {
        <<stub>>
        +void SliderChanged(object, ValueChangedEventArgs)
    }

    class AlertPage {
        +AlertPage(AlertViewModel)
        +void MaintenanceClicked(object, EventArgs)
    }

    class ResolvedAlertsPage {
        +ResolvedAlertsPage(ResolvedAlertsViewModel)
    }

    BaseEntry <|-- LogEntry
    BaseEntry <|-- ErrorEntry
    BaseViewModel <|-- LogViewModel
    BaseViewModel <|-- ErrorViewModel
    LogViewModel --> LogEntry : Uses
    ErrorViewModel --> ErrorEntry : Uses
    AlertViewModel --> AlertDbContext : Uses
    ResolvedAlertsViewModel --> AlertDbContext : Uses
    AlertDbContext --> Alert : Contains
    AlertPage --> AlertViewModel : Binds
    ResolvedAlertsPage --> ResolvedAlertsViewModel : Binds
    MaintenancePage --> AlertViewModel : Navigates to
    LogViewModel --> IUserDialogService : Uses
    ErrorViewModel --> IUserDialogService : Uses
