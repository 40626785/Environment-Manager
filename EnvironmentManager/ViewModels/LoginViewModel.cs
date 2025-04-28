using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Exceptions;

namespace EnvironmentManager.ViewModels;

/// <summary>
/// Code-behind the login page
/// </summary>
public class LoginViewModel : ObservableObject, IErrorHandling
{
    private IAuthenticationService _authentication;  
    private ILoginNavService _loginNav;
    private string _providedUsername;
    private string _providedPassword;
    private string _displayError;
    
    public ICommand Login { get; }
    public string ProvidedUsername
    {
        get => _providedUsername;
        set
        {
            if (value != _displayError)
            {
                _providedUsername = value;
                OnPropertyChanged();
            }
        }
    } 
    public string ProvidedPassword
    {
        get => _providedPassword;
        set
        {
            if (value != _displayError)
            {
                _providedPassword = value;
                OnPropertyChanged();
            }
        }
    } 
    public string DisplayError 
    {
        get => _displayError;
        set
        {
            if (value != _displayError)
            {
                _displayError = value;
                OnPropertyChanged();
            }
        }
    } 

    public LoginViewModel(IAuthenticationService authentication, ILoginNavService loginNav)
    {
        Trace.WriteLine("LoginViewModel constructor executing.");
        _authentication = authentication;
        _loginNav = loginNav;
        Login = new Command(Authenticate);
    }

    /// <summary>
    /// Pass user-provided credentials to authentication function.
    /// 
    /// Upon success, calls function to route to main app content.
    /// 
    /// Handles failed authentication.
    /// </summary>
    public void Authenticate()
    {
        try
        {
            _authentication.Authenticate(_providedUsername, _providedPassword);
            if(_authentication.Authenticated)
            {
                _loginNav.RouteOnLogin(); //allow app access upon successful login
            }
        }
        catch(LoginException e)
        {
            HandleError(e, "Incorrect Username or Password"); //handle incorrect credentials
        }
        catch(Exception e)
        {
            HandleError(e, "Unexpected Error Occurred"); //handle other exceptions
        } 
    }

    /// <summary>
    /// Write exception to Trace and set display property to show supplied message in application
    /// </summary>
    /// <param name="e"></param>
    /// <param name="message"></param>
    public void HandleError(Exception e, string message)
    {
        // Log the full exception details for better debugging
        string logMessage = $"Error in LoginViewModel: {message}\\n" +
                            $"Exception Type: {e.GetType().FullName}\\n" +
                            $"Exception Message: {e.Message}\\n" +
                            $"Stack Trace: {e.StackTrace}";
        
        if (e.InnerException != null)
        {
            logMessage += $"\\nInner Exception Type: {e.InnerException.GetType().FullName}\\n" +
                          $"Inner Exception Message: {e.InnerException.Message}\\n" +
                          $"Inner Exception Stack Trace: {e.InnerException.StackTrace}";
        }

        Trace.WriteLine(logMessage);
        Console.WriteLine(logMessage);

        DisplayError = message;
    }
}
