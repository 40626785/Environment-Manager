classDiagram

class Alert {
    int AlertId
    int LocationId
    DateTime Date_Time
    string Parameter
    double? Value
    double? Deviation
    string Message
    DateTime CreatedAt
    bool IsResolved
}

class AlertDbContext {
    DbSet<Alert> AlertTable
    + AlertDbContext(options: DbContextOptions<AlertDbContext>)
}

class AlertViewModel {
    IDbContextFactory<AlertDbContext> _dbContextFactory
    ObservableCollection<Alert> ActiveAlerts
    + LoadActiveAlerts(): Task
    + MarkAsResolved(alertId: int): Task
    + ViewAllResolvedAlerts(): Task
}

class ResolvedAlertsViewModel {
    IDbContextFactory<AlertDbContext> _dbContextFactory
    ObservableCollection<Alert> ResolvedAlerts
    + LoadResolvedAlerts(): Task
    + DeleteResolvedAlert(alertId: int): Task
}

class AlertPage {
    + MaintenanceClicked(sender: object, e: EventArgs): Task
}

class ResolvedAlertsPage {
    + ResolvedAlertsPage(viewModel: ResolvedAlertsViewModel)
}

AlertDbContext --> Alert : contains
AlertViewModel --> AlertDbContext : uses
ResolvedAlertsViewModel --> AlertDbContext : uses
AlertViewModel --> Alert : manages
ResolvedAlertsViewModel --> Alert : manages
AlertPage --> AlertViewModel : binds
ResolvedAlertsPage --> ResolvedAlertsViewModel : binds
