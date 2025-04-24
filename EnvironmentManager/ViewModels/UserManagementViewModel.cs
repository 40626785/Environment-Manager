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

namespace EnvironmentManager.ViewModels;

public class UserManagementViewModel : BaseViewModel, IErrorHandling
{
    private readonly IUserManagementDataStore _userStore;
    private readonly ISessionService _sessionService;
    private readonly IRunOnMainThread _mainThread;
    
    private User _selectedUser;
    private string _searchQuery;
    private bool _isUserSelected;
    private bool _isAdministrator;
    private string _displayError;
    
    public ObservableCollection<User> Users { get; private set; }
    
    public ICommand SearchCommand { get; }
    public ICommand AddUserCommand { get; }
    public ICommand EditUserCommand { get; }
    public ICommand DeleteUserCommand { get; }
    public ICommand UserSelectedCommand { get; }
    
    public string DisplayError
    {
        get => _displayError;
        set
        {
            if (_displayError != value)
            {
                _displayError = value;
                OnPropertyChanged();
            }
        }
    }
    
    public User SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (_selectedUser != value)
            {
                _selectedUser = value;
                OnPropertyChanged();
                IsUserSelected = _selectedUser != null;
            }
        }
    }
    
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (_searchQuery != value)
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }
    }
    
    public bool IsUserSelected
    {
        get => _isUserSelected;
        set
        {
            if (_isUserSelected != value)
            {
                _isUserSelected = value;
                OnPropertyChanged();
            }
        }
    }
    
    public bool IsAdministrator
    {
        get => _isAdministrator;
        set
        {
            if (_isAdministrator != value)
            {
                _isAdministrator = value;
                OnPropertyChanged();
            }
        }
    }
    
    public UserManagementViewModel(IUserManagementDataStore userStore, ISessionService sessionService, IRunOnMainThread mainThread)
    {
        _userStore = userStore;
        _sessionService = sessionService;
        _mainThread = mainThread;
        
        Users = new ObservableCollection<User>();
        
        SearchCommand = new Command(ExecuteSearch);
        AddUserCommand = new Command(ExecuteAddUser);
        EditUserCommand = new Command<User>(ExecuteEditUser);
        DeleteUserCommand = new Command<User>(ExecuteDeleteUser);
        UserSelectedCommand = new Command(ExecuteUserSelected);
        
        // Check if the current user is an administrator
        CheckAdministratorPermissions();
    }
    
    private void CheckAdministratorPermissions()
    {
        try
        {
            var currentUser = _sessionService.AuthenticatedUser;
            
            // Check for Administrator role in any format (enum value or string representations)
            bool isAdmin = false;
            
            if (currentUser != null)
            {
                // Check enum value directly
                isAdmin = currentUser.Role == Roles.Administrator;
                
                // Also check the role value as integer (0 = Administrator)
                if (!isAdmin)
                {
                    isAdmin = (int)currentUser.Role == 0;
                }
            }
            
            IsAdministrator = isAdmin;
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error checking user permissions");
        }
    }
    
    public void LoadUsers()
    {
        if (IsBusy)
            return;
        
        IsBusy = true;
        
        try
        {
            var users = _userStore.GetAllUsers();
            
            _mainThread.RunMainThread(() =>
            {
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            });
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error loading users");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private void ExecuteSearch()
    {
        if (IsBusy)
            return;
        
        IsBusy = true;
        
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
            
            _mainThread.RunMainThread(() =>
            {
                Users.Clear();
                foreach (var user in filteredUsers)
                {
                    Users.Add(user);
                }
            });
        }
        catch (Exception ex)
        {
            HandleError(ex, "Error searching users");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private async void ExecuteAddUser()
    {
        // Create a view model for adding a new user
        var viewModel = new EditUserViewModel(_userStore, _mainThread, () => 
        {
            // Callback when adding is complete
            LoadUsers();
            Application.Current.MainPage.Navigation.PopAsync();
        });
        
        // Navigate to the edit page
        var page = new EditUserPage(viewModel);
        await Application.Current.MainPage.Navigation.PushAsync(page);
    }
    
    private async void ExecuteEditUser(User user)
    {
        if (user == null)
            return;
        
        // Create a view model for editing the user
        var viewModel = new EditUserViewModel(_userStore, _mainThread, user, () => 
        {
            // Callback when editing is complete
            LoadUsers();
            Application.Current.MainPage.Navigation.PopAsync();
        });
        
        // Navigate to the edit page
        var page = new EditUserPage(viewModel);
        await Application.Current.MainPage.Navigation.PushAsync(page);
    }
    
    private async void ExecuteDeleteUser(User user)
    {
        if (user == null)
            return;
        
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete", 
            $"Are you sure you want to delete user {user.Username}?", 
            "Yes", "No");
        
        if (confirm)
        {
            try
            {
                await _userStore.DeleteUser(user);
                
                _mainThread.RunMainThread(() =>
                {
                    Users.Remove(user);
                    
                    if (SelectedUser == user)
                    {
                        SelectedUser = null;
                    }
                });
                
                await Application.Current.MainPage.DisplayAlert("Success", "User deleted successfully", "OK");
            }
            catch (Exception ex)
            {
                HandleError(ex, "Error deleting user");
            }
        }
    }
    
    private void ExecuteUserSelected()
    {
        // This method is triggered when a user is selected in the collection view
        // The SelectedUser property is already updated by the binding
    }
    
    public void HandleError(Exception e, string message)
    {
        Trace.WriteLine($"Error in UserManagementViewModel: {e.Message}");
        _mainThread.RunMainThread(async () =>
        {
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        });
    }
} 