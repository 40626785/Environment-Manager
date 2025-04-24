```mermaid
classDiagram
    UserManagementDbContext -- User : manages
    UserManagementDbContext -- Role : references
    IUserManagementDataStore <|.. UserManagementDataStore : implements
    UserManagementDataStore -- UserManagementDbContext : uses
    UserManagementDataStore -- IUserLogService : uses
    IUserLogService <|.. UserLogService : implements
    UserManagementViewModel -- IUserManagementDataStore : uses
    UserManagementViewModel -- ISessionService : references
    UserManagementViewModel -- IRunOnMainThread : uses
    EditUserViewModel -- IUserManagementDataStore : uses
    EditUserViewModel -- IRunOnMainThread : uses
    UserManagementViewModel -- User : manages
    EditUserViewModel -- User : edits
    User -- Role : has
    
    class User {
        +string Username
        +string Password
        -Roles _role
        +Roles Role
        +int DatabaseRoleId
        +User()
    }
    
    class Roles {
        <<enumeration>>
        Administrator
        EnvironmentalScientist
        OperationsManager
    }
    
    class Role {
        +int RoleId
        +string RoleName
        +string Description
        +DateTime CreatedDate
        +DateTime LastModifiedDate
    }
    
    class UserLog {
        +int Id
        +string Username
        +string ActionType
        +string ChangedFields
        +string OldValues
        +string NewValues
        +string PerformedBy
        +DateTime Timestamp
    }
    
    class UserManagementDbContext {
        +DbSet~User~ Users
        +DbSet~Role~ Roles
        +UserManagementDbContext(DbContextOptions~UserManagementDbContext~ options)
        #void OnModelCreating(ModelBuilder modelBuilder)
    }
    
    class IUserManagementDataStore {
        <<interface>>
        +IEnumerable~User~ GetAllUsers()
        +User GetUser(string username)
        +IEnumerable~User~ SearchUsers(string searchQuery)
        +IEnumerable~Role~ GetAllRoles()
        +Role GetRole(int roleId)
        +Task~User~ CreateUser(User user)
        +Task~User~ UpdateUser(User user)
        +Task~bool~ DeleteUser(User user)
        +Task~Role~ CreateRole(Role role)
        +Task~Role~ UpdateRole(Role role)
        +Task~bool~ DeleteRole(int roleId)
    }
    
    class UserManagementDataStore {
        -UserManagementDbContext _context
        -IUserLogService _logService
        +UserManagementDataStore(UserManagementDbContext context, IUserLogService logService)
        +IEnumerable~User~ GetAllUsers()
        +User GetUser(string username)
        +IEnumerable~User~ SearchUsers(string query)
        +IEnumerable~Role~ GetAllRoles()
        +Role GetRole(int roleId)
        +Task~User~ CreateUser(User user)
        +Task~User~ UpdateUser(User user)
        +Task~bool~ DeleteUser(User user)
        +Task~Role~ CreateRole(Role role)
        +Task~Role~ UpdateRole(Role role)
        +Task~bool~ DeleteRole(int roleId)
    }
    
    class IUserLogService {
        <<interface>>
        +Task LogUserCreatedAsync(User user)
        +Task LogUserUpdatedAsync(User oldUser, User newUser)
        +Task LogUserDeletedAsync(User user)
    }
    
    class UserLogService {
        -UserManagementDbContext _context
        +UserLogService(UserManagementDbContext context)
        +Task LogUserCreatedAsync(User user)
        +Task LogUserUpdatedAsync(User oldUser, User newUser)
        +Task LogUserDeletedAsync(User user)
        -Task AddLogEntryAsync(string username, string actionType, string? changedFields, string? oldValues, string? newValues)
    }
    
    class UserManagementViewModel {
        -IUserManagementDataStore _userStore
        -ISessionService _sessionService
        -IRunOnMainThread _mainThread
        -User _selectedUser
        -string _searchQuery
        -bool _isUserSelected
        -bool _isAdministrator
        -string _displayError
        +ObservableCollection~User~ Users
        +ICommand SearchCommand
        +ICommand AddUserCommand
        +ICommand EditUserCommand
        +ICommand DeleteUserCommand
        +ICommand UserSelectedCommand
        +string DisplayError
        +User SelectedUser
        +string SearchQuery
        +bool IsUserSelected
        +bool IsAdministrator
        +UserManagementViewModel(IUserManagementDataStore userStore, ISessionService sessionService, IRunOnMainThread mainThread)
        -void CheckAdministratorPermissions()
        +void LoadUsers()
        -void ExecuteSearch()
        -void ExecuteAddUser()
        -void ExecuteEditUser(User user)
        -void ExecuteDeleteUser(User user)
        -void ExecuteUserSelected()
        +void HandleError(Exception e, string message)
    }
    
    class EditUserViewModel {
        -IUserManagementDataStore _userStore
        -IRunOnMainThread _mainThread
        -Action _onComplete
        -User? _originalUser
        -string _username
        -string _password
        -Roles _selectedRole
        -bool _isNewUser
        -string _errorMessage
        -bool _hasError
        +ObservableCollection~Roles~ AvailableRoles
        +ICommand SaveCommand
        +ICommand CancelCommand
        +string Username
        +string Password
        +Roles SelectedRole
        +bool IsNewUser
        +string ErrorMessage
        +bool HasError
        +EditUserViewModel(IUserManagementDataStore userStore, IRunOnMainThread mainThread, Action onComplete)
        +EditUserViewModel(IUserManagementDataStore userStore, IRunOnMainThread mainThread, User userToEdit, Action onComplete)
        -void InitializeRoles()
        -void ExecuteSave()
        -void ExecuteCancel()
        -bool ValidateInput()
    }
    
    class IRunOnMainThread {
        <<interface>>
        +void RunMainThread(Action action)
    }
    
    class ISessionService {
        <<interface>>
        +User AuthenticatedUser
    }
} 