namespace EnvironmentManager.Test;

using EnvironmentManager.Models;
using EnvironmentManager.Services;
using EnvironmentManager.Interfaces;
using Moq;
using EnvironmentManager.Exceptions;
using System.Reflection;

public class SessionServiceTests
{
    [Fact]
    public async Task NewSession_Timeout_LogoutCalled()
    {
        //Arrange
        var loginNavService = new Mock<ILoginNavService>();
        var mainThread = new Mock<IRunOnMainThread>(); //main thread operation abstracted into interface as not testable
        mainThread.Setup(x => x.RunMainThread(It.IsAny<Action>())).Callback<Action>(action => action());
        User user = new User();
        user.Username = "test";
        //Using reflection to overwrite private ttl value to change delay to a value that is more test-friendly
        SessionService sessionService = new SessionService(loginNavService.Object, mainThread.Object);
        var ttl = typeof(SessionService).GetField("_ttl", BindingFlags.NonPublic | BindingFlags.Instance);
        ttl.SetValue(sessionService, 20);
        //Action
        sessionService.NewSession(user);
        TimeSpan waitPeriod = sessionService.Expiry.Value - DateTime.Now; //use of .Value is safe here as we know the value is not null
        int delay = (int) waitPeriod.TotalMilliseconds;
        await Task.Delay(delay);
        //Assert
        Assert.Equal(user, sessionService.AuthenticatedUser);
        loginNavService.Verify(x => x.RouteOnLogout(), Times.Once);
    }
}