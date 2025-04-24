using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using EnvironmentManager.Services;
using EnvironmentManager.ViewModels;
using Moq;
using System.Collections.ObjectModel;

namespace EnvironmentManager.Test;

public class EditUserViewModelTests
{
    [Fact]
    public void CreateMode_InitializesCorrectly()
    {
        // Arrange
        var userStoreMock = new Mock<IUserManagementDataStore>();
        var mainThreadMock = new Mock<IRunOnMainThread>();
        bool onCompleteCalled = false;
        Action onComplete = () => onCompleteCalled = true;
        
        // Act
        var viewModel = new EditUserViewModel(userStoreMock.Object, mainThreadMock.Object, onComplete);
        
        // Assert
        Assert.True(viewModel.IsNewUser);
        Assert.Equal(string.Empty, viewModel.Username);
        Assert.Equal(string.Empty, viewModel.Password);
        Assert.Equal(Roles.EnvironmentalScientist, viewModel.SelectedRole);
        Assert.NotNull(viewModel.AvailableRoles);
        Assert.Contains(viewModel.AvailableRoles, r => r == Roles.Administrator);
        Assert.Contains(viewModel.AvailableRoles, r => r == Roles.EnvironmentalScientist);
        Assert.Contains(viewModel.AvailableRoles, r => r == Roles.OperationsManager);
    }
    
    [Fact]
    public void EditMode_InitializesCorrectly()
    {
        // Arrange
        var userStoreMock = new Mock<IUserManagementDataStore>();
        var mainThreadMock = new Mock<IRunOnMainThread>();
        bool onCompleteCalled = false;
        Action onComplete = () => onCompleteCalled = true;
        
        // Create a user to edit
        var user = new User 
        { 
            Username = "testuser", 
            Password = "oldpassword", 
            Role = Roles.Administrator 
        };
        
        // Act
        var viewModel = new EditUserViewModel(userStoreMock.Object, mainThreadMock.Object, user, onComplete);
        
        // Assert
        Assert.False(viewModel.IsNewUser);
        Assert.Equal("testuser", viewModel.Username);
        Assert.Equal(string.Empty, viewModel.Password); // Password should not be populated in edit mode
        Assert.Equal(Roles.Administrator, viewModel.SelectedRole);
        Assert.NotNull(viewModel.AvailableRoles);
        Assert.Contains(viewModel.AvailableRoles, r => r == Roles.Administrator);
    }
    
    [Fact]
    public void ErrorMessage_SetsHasErrorFlag()
    {
        // Arrange
        var userStoreMock = new Mock<IUserManagementDataStore>();
        var mainThreadMock = new Mock<IRunOnMainThread>();
        var viewModel = new EditUserViewModel(userStoreMock.Object, mainThreadMock.Object, () => { });
        
        // Initial state
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
        Assert.False(viewModel.HasError);
        
        // Act - set error message
        viewModel.ErrorMessage = "Test error message";
        
        // Assert
        Assert.Equal("Test error message", viewModel.ErrorMessage);
        Assert.True(viewModel.HasError);
        
        // Act - clear error message
        viewModel.ErrorMessage = string.Empty;
        
        // Assert
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
        Assert.False(viewModel.HasError);
    }
    
    [Fact]
    public void CancelCommand_InvokesOnComplete()
    {
        // Arrange
        var userStoreMock = new Mock<IUserManagementDataStore>();
        var mainThreadMock = new Mock<IRunOnMainThread>();
        bool onCompleteCalled = false;
        Action onComplete = () => onCompleteCalled = true;
        
        var viewModel = new EditUserViewModel(userStoreMock.Object, mainThreadMock.Object, onComplete);
        
        // Act
        viewModel.CancelCommand.Execute(null);
        
        // Assert
        Assert.True(onCompleteCalled);
    }
    
    [Fact]
    public void Username_PropertyChangeWorks()
    {
        // Arrange
        var userStoreMock = new Mock<IUserManagementDataStore>();
        var mainThreadMock = new Mock<IRunOnMainThread>();
        var viewModel = new EditUserViewModel(userStoreMock.Object, mainThreadMock.Object, () => { });
        
        bool propertyChanged = false;
        string changedPropertyName = null;
        
        viewModel.PropertyChanged += (sender, args) => 
        {
            propertyChanged = true;
            changedPropertyName = args.PropertyName;
        };
        
        // Act
        viewModel.Username = "newusername";
        
        // Assert
        Assert.Equal("newusername", viewModel.Username);
        Assert.True(propertyChanged);
        Assert.Equal(nameof(EditUserViewModel.Username), changedPropertyName);
    }
    
    [Fact]
    public void Password_PropertyChangeWorks()
    {
        // Arrange
        var userStoreMock = new Mock<IUserManagementDataStore>();
        var mainThreadMock = new Mock<IRunOnMainThread>();
        var viewModel = new EditUserViewModel(userStoreMock.Object, mainThreadMock.Object, () => { });
        
        bool propertyChanged = false;
        string changedPropertyName = null;
        
        viewModel.PropertyChanged += (sender, args) => 
        {
            propertyChanged = true;
            changedPropertyName = args.PropertyName;
        };
        
        // Act
        viewModel.Password = "newpassword";
        
        // Assert
        Assert.Equal("newpassword", viewModel.Password);
        Assert.True(propertyChanged);
        Assert.Equal(nameof(EditUserViewModel.Password), changedPropertyName);
    }
    
    [Fact]
    public void SelectedRole_PropertyChangeWorks()
    {
        // Arrange
        var userStoreMock = new Mock<IUserManagementDataStore>();
        var mainThreadMock = new Mock<IRunOnMainThread>();
        var viewModel = new EditUserViewModel(userStoreMock.Object, mainThreadMock.Object, () => { });
        
        bool propertyChanged = false;
        string changedPropertyName = null;
        
        viewModel.PropertyChanged += (sender, args) => 
        {
            propertyChanged = true;
            changedPropertyName = args.PropertyName;
        };
        
        // Act
        viewModel.SelectedRole = Roles.Administrator;
        
        // Assert
        Assert.Equal(Roles.Administrator, viewModel.SelectedRole);
        Assert.True(propertyChanged);
        Assert.Equal(nameof(EditUserViewModel.SelectedRole), changedPropertyName);
    }
} 