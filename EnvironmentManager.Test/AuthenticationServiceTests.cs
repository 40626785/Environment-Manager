namespace EnvironmentManager.Test;

using EnvironmentManager.Models;
using EnvironmentManager.Services;
using EnvironmentManager.Interfaces;
using Moq;
using EnvironmentManager.Exceptions;

public class AuthenticationServiceTests
{
    [Fact]
    public void Authenticate_Success_CreateSession()
    {
        //Arrange
        var dataStore = new Mock<IUserDataStore>();
        var sessionManager = new Mock<ISessionService>();
        var testUser = new User();
        string username = "username";
        string password = "password";
        testUser.Username = username;
        testUser.Password = password;
        dataStore.Setup(x => x.GetUser(username)).Returns(testUser);
        var authService = new AuthenticationService(dataStore.Object, sessionManager.Object);
        //Action
        authService.Authenticate(username, password);
        //Assert
        Assert.True(authService.Authenticated);
        Assert.Equal(testUser, authService.AuthenticatedUser);
        sessionManager.Verify(x => x.NewSession(testUser), Times.Once);
    }
    [Fact]
    public void Authenticate_IncorrectDetails_LoginException()
    {
        //Arrange
        var dataStore = new Mock<IUserDataStore>();
        var sessionManager = new Mock<ISessionService>();
        var testUser = new User();
        string username = "username";
        string password = "password";
        testUser.Username = username;
        testUser.Password = password;
        dataStore.Setup(x => x.GetUser(username)).Returns(testUser);
        var authService = new AuthenticationService(dataStore.Object, sessionManager.Object);
        //Assert
        Assert.Throws<LoginException>(() => authService.Authenticate(username, "incorrect"));
        Assert.False(authService.Authenticated);
    }
    [Fact]
    public void Authenticate_InvalidUser_LoginException()
    {
        //Arrange
        var dataStore = new Mock<IUserDataStore>();
        var sessionManager = new Mock<ISessionService>();
        dataStore.Setup(x => x.GetUser("incorrect")).Returns(new User());
        var authService = new AuthenticationService(dataStore.Object, sessionManager.Object);
        //Assert
        Assert.Throws<LoginException>(() => authService.Authenticate("incorrect", "incorrect"));
        Assert.False(authService.Authenticated);
    }
}