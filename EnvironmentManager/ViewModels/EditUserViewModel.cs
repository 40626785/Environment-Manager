using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EnvironmentManager.ViewModels;

public class EditUserViewModel : BaseViewModel
{
    private readonly IUserManagementDataStore _userStore;
    private readonly IRunOnMainThread _mainThread;
    private readonly Action _onComplete;
    
    private User? _originalUser;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private Roles _selectedRole;
    private bool _isNewUser;
    private string _errorMessage = string.Empty;
    private bool _hasError;
    
    public ObservableCollection<Roles> AvailableRoles { get; private set; }
    
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    
    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged();
            }
        }
    }
    
    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged();
            }
        }
    }
    
    public Roles SelectedRole
    {
        get => _selectedRole;
        set
        {
            if (_selectedRole != value)
            {
                _selectedRole = value;
                OnPropertyChanged();
            }
        }
    }
    
    public bool IsNewUser
    {
        get => _isNewUser;
        set
        {
            if (_isNewUser != value)
            {
                _isNewUser = value;
                OnPropertyChanged();
            }
        }
    }
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged();
                HasError = !string.IsNullOrEmpty(value);
            }
        }
    }
    
    public bool HasError
    {
        get => _hasError;
        set
        {
            if (_hasError != value)
            {
                _hasError = value;
                OnPropertyChanged();
            }
        }
    }
    
    // Constructor for creating a new user
    public EditUserViewModel(IUserManagementDataStore userStore, IRunOnMainThread mainThread, Action onComplete)
    {
        _userStore = userStore;
        _mainThread = mainThread;
        _onComplete = onComplete;
        
        IsNewUser = true;
        _selectedRole = Roles.BasicUser; // Most restrictive role as default for security
        
        InitializeRoles();
        
        SaveCommand = new Command(ExecuteSave);
        CancelCommand = new Command(ExecuteCancel);
    }
    
    // Constructor for editing an existing user
    public EditUserViewModel(IUserManagementDataStore userStore, IRunOnMainThread mainThread, User userToEdit, Action onComplete)
    {
        _userStore = userStore;
        _mainThread = mainThread;
        _onComplete = onComplete;
        _originalUser = userToEdit;
        
        IsNewUser = false;
        Username = userToEdit.Username ?? string.Empty;
        Password = string.Empty; // Don't show the current password
        SelectedRole = userToEdit.Role;
        
        InitializeRoles();
        
        SaveCommand = new Command(ExecuteSave);
        CancelCommand = new Command(ExecuteCancel);
    }
    
    private void InitializeRoles()
    {
        // Create a collection of available roles
        AvailableRoles = new ObservableCollection<Roles>
        {
            Roles.BasicUser,
            Roles.Administrator,
            Roles.EnvironmentalScientist,
            Roles.OperationsManager
        };
    }
    
    private async void ExecuteSave()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Username is required";
            return;
        }
        
        if (IsNewUser && string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Password is required for new users";
            return;
        }
        
        try
        {
            IsBusy = true;
            
            if (IsNewUser)
            {
                // Create a new user
                var newUser = new User
                {
                    Username = Username,
                    Password = Password,
                    Role = SelectedRole
                };
                
                await _userStore.CreateUser(newUser);
                await Application.Current.MainPage.DisplayAlert("Success", "User created successfully", "OK");
            }
            else if (_originalUser != null)
            {
                // Update existing user
                _originalUser.Role = SelectedRole;
                
                // Only update password if a new one was provided
                if (!string.IsNullOrEmpty(Password))
                {
                    _originalUser.Password = Password;
                }
                
                await _userStore.UpdateUser(_originalUser);
                await Application.Current.MainPage.DisplayAlert("Success", "User updated successfully", "OK");
            }
            
            // Return to the previous page
            _onComplete?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving user: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private void ExecuteCancel()
    {
        // Return to the previous page without saving
        _onComplete?.Invoke();
    }
} 