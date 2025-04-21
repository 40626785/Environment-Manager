using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Exceptions;

namespace EnvironmentManager.ViewModels;

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
        _authentication = authentication;
        _loginNav = loginNav;
        Login = new Command(Authenticate);
    }

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

    public void HandleError(Exception e, string message)
    {
        Trace.WriteLine(e.Message);
        DisplayError = message;
    }
}
