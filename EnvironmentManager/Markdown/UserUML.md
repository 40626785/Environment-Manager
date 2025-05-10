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

    DatabaseAdminViewModel --> IDatabaseAdminDataStore : Uses
    DatabaseAdminPage --> DatabaseAdminViewModel : Binds

    class User {
        +string Username
        +string Password
        +int Role
    }

    class UserDbContext {
        +DbSet<User> Users
        +UserDbContext(DbContextOptions)
        +void OnModelCreating(ModelBuilder)
    }

    class AddUserViewModel {
        +User NewUser
        +List<int> RoleOptions
        +ICommand SaveCommand
        +AddUserViewModel(UserDbContext, IUserDialogService)
        +Task SaveAsync()
    }

    class AdminUserViewModel {
        +ObservableCollection<User> TableData
        +string UsernameFilter
        +string RoleFilterText
        +ICommand LoadDataCommand
        +ICommand ApplyFiltersCommand
        +ICommand DeleteFilteredCommand
        +ICommand ExportToCsvCommand
        +ICommand ToggleFilterVisibilityCommand
        +ICommand RowTappedCommand
        +ICommand AddUserCommand
        +AdminUserViewModel(IDbContextFactory, IUserDialogService)
        +Task LoadDataAsync()
        +Task ApplyFiltersAsync()
        +Task DeleteFilteredAsync()
        +Task ExportToCsvAsync()
    }

    class AddUserPage {
        +AddUserPage(AddUserViewModel)
    }

    class AdminUserPage {
        +AdminUserPage(AdminUserViewModel)
        +void OnAppearing()
    }

    AddUserViewModel --> UserDbContext : Uses
    AdminUserViewModel --> UserDbContext : Uses
    AddUserPage --> AddUserViewModel : Binds
    AdminUserPage --> AdminUserViewModel : Binds
    UserDbContext --> User : Contains
    AdminUserViewModel --> User : Manages
    AddUserViewModel --> User : Creates

    %% Navigation connections
    AdminUserPage --> AddUserPage : Navigates to
    DatabaseAdminPage --> AdminUserPage : Navigates to
