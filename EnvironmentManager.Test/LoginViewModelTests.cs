namespace EnvironmentManager.Test;

using System.Diagnostics;
using System.Text;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Interfaces;
using Moq;
using EnvironmentManager.Exceptions;

public class LoginViewModelTests
{
    [Fact]
    public void Authenticate_UnexpectedError_CatchException()
    {
        // Arrange
        var authService = new Mock<IAuthenticationService>();
        authService.Setup(x => x.Authenticate(It.IsAny<string>(),It.IsAny<string>())).Throws(new Exception());
        var loginNav = new Mock<ILoginNavService>();
        LoginViewModel viewModel = new LoginViewModel(authService.Object, loginNav.Object);
        // Action
        viewModel.Authenticate();
        //Assert
        Assert.NotNull(viewModel.DisplayError);
    }
    [Fact]
    public void Authenticate_IncorrectCredentials_CatchException()
    {
        // Arrange
        var authService = new Mock<IAuthenticationService>();
        authService.Setup(x => x.Authenticate(It.IsAny<string>(),It.IsAny<string>())).Throws(new LoginException());
        var loginNav = new Mock<ILoginNavService>();
        LoginViewModel viewModel = new LoginViewModel(authService.Object, loginNav.Object);
        // Action
        viewModel.Authenticate();
        //Assert
        Assert.NotNull(viewModel.DisplayError);
    }
    [Fact]
    public void Authenticate_Success_RouteToMain()
    {
        // Arrange
        var authService = new Mock<IAuthenticationService>();
        authService.SetupGet(x => x.Authenticated).Returns(true);
        var loginNav = new Mock<ILoginNavService>();
        LoginViewModel viewModel = new LoginViewModel(authService.Object, loginNav.Object);
        // Action
        viewModel.Authenticate();
        //Assert
        authService.Verify(x => x.Authenticate(It.IsAny<string>(),It.IsAny<string>()), Times.Once);
        loginNav.Verify(x => x.RouteOnLogin(), Times.Once);
    }
    [Fact]
    public void HandleError_Store()
    {
        // Arrange
        var authService = new Mock<IAuthenticationService>();
        var loginNav = new Mock<ILoginNavService>();
        LoginViewModel viewModel = new LoginViewModel(authService.Object, loginNav.Object);
        // Action
        string exceptionMessage = "test exception";
        string errorMessage = "test message";
        try
        {
            throw new Exception(exceptionMessage);
        }
        catch(Exception e)
        {
            viewModel.HandleError(e, errorMessage);
        }
        //Assert
        Assert.Equal(viewModel.DisplayError, errorMessage);
    }

    [Fact]
    public void HandleError_Trace()
    {
        // Arrange
        var authService = new Mock<IAuthenticationService>();
        var loginNav = new Mock<ILoginNavService>();
        LoginViewModel viewModel = new LoginViewModel(authService.Object, loginNav.Object);
        StringBuilder builder = new StringBuilder();
        StringWriter writer = new StringWriter(builder);
        TextWriterTraceListener listener = new TextWriterTraceListener(writer);
        Trace.Listeners.Add(listener);
        
        // Action
        string exceptionMessage = "test exception";
        string errorMessage = "test message";
        try
        {
            throw new Exception(exceptionMessage);
        }
        catch(Exception e)
        {
            viewModel.HandleError(e, errorMessage);
        }
        //Assert
        string traceContents = builder.ToString().Replace("\n","");
        Assert.Equal(exceptionMessage, traceContents);
    }
}