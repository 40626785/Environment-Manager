# User Management

## Overview

The user management system is a foundational component of the Environment Manager application, enabling administrators to create, edit, and manage user accounts with appropriate access roles. This page documents the user management-related files in the project and their roles in implementing this functionality.

## Related User Story

This feature addresses the need for secure and flexible user access management within environmental monitoring systems. It allows administrators to control who can access the system and what level of permissions they have.

[Link to GitHub Issue](https://github.com/40626785/Environment-Manager/issues/2)

## User Management Files

### Models

#### `Models/User.cs`

This file defines the User entity model that represents user accounts in the system.

**Key Features:**
- Defines properties for user identification (Username)
- Stores security information (Password)
- Manages role-based access control through the Role property
- Implements proper database mapping with attribute annotations
- Uses the Roles enum to define available user roles

#### `Models/Roles.cs`

This file defines the available user roles as an enum.

**Key Features:**
- Defines hierarchical role levels (Administrator, EnvironmentalScientist, OperationsManager)
- Provides a centralized definition of all application roles
- Used for role-based access control throughout the application

#### `Models/Role.cs`

This file defines the database representation of roles.

**Key Features:**
- Stores role metadata (RoleName, Description)
- Tracks creation and modification dates
- Maps to the Roles enum for application logic

### Data Access

#### `Data/UserManagementDbContext.cs`

This file implements the database context for user-related entities using Entity Framework Core.

**Key Features:**
- Defines DbSet for Users and Roles
- Configures entity relationships in OnModelCreating
- Follows repository pattern principles

#### `Data/UserManagementDataStore.cs`

This file implements the IUserManagementDataStore interface, providing data access operations for user management.

**Key Features:**
- Implements CRUD operations for user accounts (GetAllUsers, CreateUser, UpdateUser, DeleteUser)
- Provides search functionality for users
- Manages role operations
- Includes audit logging for user changes

### Services

#### `Services/UserLogService.cs`

This file implements the IUserLogService interface for tracking user changes.

**Key Features:**
- Logs user creation, modification, and deletion events
- Records changes to user properties
- Maintains audit trail for security and compliance

### ViewModels

#### `ViewModels/UserManagementViewModel.cs`

This ViewModel manages the collection of users and provides operations for the user management page.

**Key Features:**
- Implements MVVM pattern with BaseViewModel inheritance
- Provides LoadUsers method to retrieve users from the data store
- Includes search functionality with SearchCommand
- Manages user creation, editing, and deletion
- Handles administrator permission checks
- Uses dependency injection for services

#### `ViewModels/EditUserViewModel.cs`

This ViewModel handles the creation and editing of user accounts.

**Key Features:**
- Manages form data for creating and editing users
- Loads available roles for selection
- Implements error handling and validation
- Provides Save and Cancel commands
- Uses dependency injection for data store access

### Views

#### `Views/UserManagementPage.xaml` and `Views/UserManagementPage.xaml.cs`

These files implement the user management UI and code-behind.

**Key Features:**
- Displays the collection of users with role indicators
- Provides search functionality
- Includes add, edit, and delete options
- Shows visual role indicators with color-coding
- Binds to the UserManagementViewModel

#### `Views/EditUserPage.xaml` and `Views/EditUserPage.xaml.cs`

These files implement the UI for adding and editing users.

**Key Features:**
- Provides form fields for username, password, and role
- Displays validation errors
- Includes save and cancel buttons
- Adapts to create or edit mode
- Binds to the EditUserViewModel

## Data Flow

1. When the UserManagementPage is loaded, it initializes a UserManagementViewModel
2. The ViewModel loads all users from the UserManagementDataStore
3. The UI displays the current users with their roles and appropriate visual indicators
4. Administrators can add, edit, or delete users through the UI
5. Changes are persisted to the database and logged for audit purposes

## Unit Tests

The user management functionality is thoroughly tested with unit tests:

### `EnvironmentManager.Test/UserManagementViewModelTests.cs`

Tests the user management functionality.

**Key Tests:**
- Loading users from the data store
- Searching and filtering users
- Administrator permission checks
- User selection behavior
- User deletion

### `EnvironmentManager.Test/EditUserViewModelTests.cs`

Tests the user creation and editing functionality.

**Key Tests:**
- Initialization in create mode
- Initialization in edit mode
- Error handling
- Property change notifications
- Command behavior

## Design Patterns and Principles

- **Repository Pattern**: UserManagementDataStore abstracts data access logic
- **MVVM Pattern**: Clear separation between UI (Views), business logic (ViewModels), and data (Models)
- **Dependency Injection**: Services and data stores are injected for better testability
- **Observer Pattern**: UI elements observe changes in the ViewModel's properties
- **Command Pattern**: User interactions are implemented as commands

## Security Considerations

- Role-based access control limits functionality based on user role
- Administrator-only access to user management functions
- Audit logging of all user modifications for compliance and security
- Password handling follows security best practices

## Performance Considerations

- Efficient data loading with appropriate filtering
- Minimal database roundtrips
- Proper resource management

## Code Conventions

- Proper exception handling and validation
- Use of interfaces for dependency injection and testability
- Consistent naming conventions following C# standards
- Clear separation of concerns across layers 