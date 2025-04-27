using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Views;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Services;
using System.Threading.Tasks;

namespace EnvironmentManager.ViewModels;

/// <summary>
/// ViewModel for the User Management page.
/// Handles displaying, searching, adding, editing, and deleting users.
/// Requires Administrator privileges for modification actions.
/// </summary>
public partial class UserManagementViewModel : BaseViewModel, IErrorHandling
{
    // Injected services
    private readonly IUserManagementDataStore _userStore;
    private readonly ISessionService _sessionService;
    private readonly IRunOnMainThread _mainThread;
    
    // Observable properties for UI binding
    [ObservableProperty]
    private User _selectedUser;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isUserSelected;

    [ObservableProperty]
    private bool _isAdministrator;

    [ObservableProperty]
    private string _displayError = string.Empty;
    
    /// <summary>
    /// Collection of users displayed in the list.
    /// </summary>
    public ObservableCollection<User> Users { get; private set; }
    
    // Commands for UI actions
    public ICommand SearchCommand { get; }
    public ICommand AddUserCommand { get; }
    public ICommand EditUserCommand { get; }
    public ICommand DeleteUserCommand { get; }
    public ICommand UserSelectedCommand { get; }
    
    /// <summary>
    /// Gets or sets the error message to display on the UI.
    /// </summary>
    public string DisplayError
    {
        get => _displayError;
        set => SetProperty(ref _displayError, value);
    }
    
    /// <summary>
    /// Gets or sets the currently selected user in the list.
    /// Updates the IsUserSelected flag.
    /// </summary>
    public User SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (SetProperty(ref _selectedUser, value))
            {
                IsUserSelected = _selectedUser != null;
            }
        }
    }
    
    /// <summary>
    /// Gets or sets the search query entered by the user.
    /// </summary>
    public string SearchQuery
    {
        get => _searchQuery;
        set => SetProperty(ref _searchQuery, value);
    }
    
    /// <summary>
    /// Gets or sets a value indicating whether a user is currently selected.
    /// Used to enable/disable edit/delete buttons.
    /// </summary>
    public bool IsUserSelected
    {
        get => _isUserSelected;
        private set => SetProperty(ref _isUserSelected, value);
    }
    
    /// <summary>
    /// Gets or sets a value indicating whether the current logged-in user is an administrator.
    /// Used to enable/disable add/edit/delete functionality.
    /// </summary>
    public bool IsAdministrator
    {
        get => _isAdministrator;
        private set => SetProperty(ref _isAdministrator, value);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UserManagementViewModel"/> class.
    /// </summary>
    /// <param name="userStore">Data store for user management operations.</param>
    /// <param name="sessionService">Service to get current session information.</param>
    /// <param name="mainThread">Service to run operations on the main UI thread.</param>
    public UserManagementViewModel(IUserManagementDataStore userStore, ISessionService sessionService, IRunOnMainThread mainThread)
    {
        _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _mainThread = mainThread ?? throw new ArgumentNullException(nameof(mainThread));
        
        Users = new ObservableCollection<User>();
        
        // Initialize commands
        SearchCommand = new Command(ExecuteSearch);
        AddUserCommand = new Command(async () => await ExecuteAddUserAsync(), () => IsAdministrator); // Only allow add if admin
        EditUserCommand = new Command<User>(async (user) => await ExecuteEditUserAsync(user), (user) => IsAdministrator && user != null); // Only allow edit if admin and user selected
        DeleteUserCommand = new Command<User>(async (user) => await ExecuteDeleteUserAsync(user), (user) => IsAdministrator && user != null); // Only allow delete if admin and user selected
        UserSelectedCommand = new Command(ExecuteUserSelected);
        
        CheckAdministratorPermissions();
        // Initial load of users
        LoadUsers(); 
    }
    
    // Checks the role of the currently logged-in user
    private void CheckAdministratorPermissions()
    {
        try
        {
            var currentUser = _sessionService.AuthenticatedUser;
            bool isAdmin = false;
            if (currentUser != null)
            {
                // Check if the user has the Administrator role (enum value 1)
                isAdmin = currentUser.Role == Roles.Administrator;
                
                 // Redundant check removed as enum comparison is sufficient and robust
                 // if (!isAdmin) { isAdmin = (int)currentUser.Role == 1; // Assuming Admin=1 based on latest Role enum } 
            }
            IsAdministrator = isAdmin;
            Debug.WriteLine($"Administrator check complete. IsAdmin: {IsAdministrator}");

            // Refresh command CanExecute status based on permission
            (AddUserCommand as Command)?.ChangeCanExecute();
            (EditUserCommand as Command)?.ChangeCanExecute();
            (DeleteUserCommand as Command)?.ChangeCanExecute();
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error checking user permissions");
            IsAdministrator = false; // Default to non-admin on error
        }
    }
    
    /// <summary>
    /// Loads the list of users from the data store.
    /// </summary>
    public void LoadUsers()
    {
        if (IsBusy) return;
        IsBusy = true;
        Debug.WriteLine("Loading users...");
        try
        {
            var users = _userStore.GetAllUsers();
            Debug.WriteLine($"Retrieved {users?.Count() ?? 0} users from data store.");

            // Use the main thread service to update the ObservableCollection
            _mainThread.RunMainThread(() =>
            {
                Users.Clear();
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        Users.Add(user);
                    }
                }
                Debug.WriteLine($"ObservableCollection updated with {Users.Count} users.");
            });
        }
        catch (Exception ex)
        {
             Debug.WriteLine($"Error loading users: {ex.Message}");
            HandleError(ex, "Error loading users");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    // Executes the search based on the SearchQuery property
    private void ExecuteSearch()
    {
        if (IsBusy) return;
        IsBusy = true;
         Debug.WriteLine($"Executing search with query: '{SearchQuery}'");
        try
        {
            IEnumerable<User> filteredUsers;
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredUsers = _userStore.GetAllUsers();
            }
            else
            {
                filteredUsers = _userStore.SearchUsers(SearchQuery);
            }
            
             Debug.WriteLine($"Search returned {filteredUsers?.Count() ?? 0} users.");
            // Update the collection on the main thread
            _mainThread.RunMainThread(() =>
            {
                Users.Clear();
                 if (filteredUsers != null)
                 {
                    foreach (var user in filteredUsers)
                    {
                        Users.Add(user);
                    }
                 }
                 Debug.WriteLine($"ObservableCollection updated with {Users.Count} search results.");
            });
        }
        catch (Exception ex)
        {
             Debug.WriteLine($"Error searching users: {ex.Message}");
            HandleError(ex, "Error searching users");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    // Navigates to the Add User page
    private async Task ExecuteAddUserAsync()
    {
         Debug.WriteLine("Navigating to Add User page.");
        // Create the ViewModel for the EditUserPage (in 'Add' mode)
        var editViewModel = new EditUserViewModel(_userStore, _mainThread, () => 
        {
            // Callback action after saving/canceling on the EditUserPage
            Debug.WriteLine("Returning from Add User page. Reloading users.");
            LoadUsers(); // Refresh the user list
            // Navigate back (assuming PushAsync was used)
            Application.Current?.MainPage?.Navigation.PopAsync();
        });
        
        var page = new EditUserPage(editViewModel);
        await Application.Current?.MainPage?.Navigation.PushAsync(page);
    }
    
    // Navigates to the Edit User page for the selected user
    private async Task ExecuteEditUserAsync(User? user)
    {
        if (user == null)
        {
            Debug.WriteLine("ExecuteEditUserAsync called with null user.");
            return;
        }
         Debug.WriteLine($"Navigating to Edit User page for user: {user.Username}");
        // Create the ViewModel for the EditUserPage (in 'Edit' mode)
        var editViewModel = new EditUserViewModel(_userStore, _mainThread, user, () => 
        {
            // Callback action after saving/canceling on the EditUserPage
             Debug.WriteLine($"Returning from Edit User page for user: {user.Username}. Reloading users.");
            LoadUsers(); // Refresh the user list
            // Navigate back
            Application.Current?.MainPage?.Navigation.PopAsync();
        });
        
        var page = new EditUserPage(editViewModel);
        await Application.Current?.MainPage?.Navigation.PushAsync(page);
    }
    
    // Deletes the selected user after confirmation
    private async Task ExecuteDeleteUserAsync(User? user)
    {
        if (user == null)
        {
            Debug.WriteLine("ExecuteDeleteUserAsync called with null user.");
            return;
        }
        
        // Prevent deleting the currently logged-in user
        if (_sessionService.AuthenticatedUser?.Username == user.Username)
        {
             Debug.WriteLine("Attempted to delete currently logged-in user.");
             await Application.Current.MainPage.DisplayAlert("Error", "You cannot delete the currently logged-in user.", "OK");
             return;
        }

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete", 
            $"Are you sure you want to delete user '{user.Username}'?", 
            "Yes", "No");
        
        if (confirm)
        {
             Debug.WriteLine($"Deletion confirmed for user: {user.Username}");
            try
            {
                await _userStore.DeleteUser(user);
                 Debug.WriteLine($"User {user.Username} deleted from data store.");
                
                // Update the collection on the main thread
                _mainThread.RunMainThread(() =>
                {
                    Users.Remove(user);
                     Debug.WriteLine($"User {user.Username} removed from ObservableCollection.");
                    if (SelectedUser == user)
                    {
                        SelectedUser = null; // Clear selection if the deleted user was selected
                    }
                    // Refresh CanExecute for commands dependent on selection
                     (EditUserCommand as Command)?.ChangeCanExecute();
                     (DeleteUserCommand as Command)?.ChangeCanExecute();
                });
                
                await Application.Current.MainPage.DisplayAlert("Success", "User deleted successfully", "OK");
            }
            catch (Exception ex)
            {
                 Debug.WriteLine($"Error deleting user {user.Username}: {ex.Message}");
                HandleError(ex, $"Error deleting user '{user.Username}'");
            }
        }
    }
    
    // Handles the selection change event from the UI (e.g., CollectionView)
    private void ExecuteUserSelected()
    {
        // Logic is handled by the SelectedUser property setter
         Debug.WriteLine($"User selected: {SelectedUser?.Username ?? "None"}");
         // Refresh CanExecute status for commands dependent on selection
         (EditUserCommand as Command)?.ChangeCanExecute();
         (DeleteUserCommand as Command)?.ChangeCanExecute();
    }
    
    /// <summary>
    /// Handles errors by logging them and displaying an alert to the user.
    /// </summary>
    /// <param name="e">The exception that occurred.</param>
    /// <param name="message">A user-friendly message to display.</param>
    public void HandleError(Exception e, string message)
    {
        // Log the full error details
        Trace.WriteLine($"Error in UserManagementViewModel: {message}");
        Trace.WriteLine($"Exception Type: {e.GetType().FullName}");
        Trace.WriteLine($"Exception Message: {e.Message}");
        Trace.WriteLine($"Stack Trace: {e.StackTrace}");
        if (e.InnerException != null)
        {
             Trace.WriteLine($"Inner Exception: {e.InnerException.Message}");
             Trace.WriteLine($"Inner Stack Trace: {e.InnerException.StackTrace}");
        }

        // Show a user-friendly message on the main thread
        _mainThread.RunMainThread(async () =>
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
            }
        });
    }
} 