namespace EnvironmentManager.Services;

using EnvironmentManager.Interfaces;
using EnvironmentManager.Views;

/// <summary>
/// Allows changing MainPage without handling within ViewModel and breaking MVVM structure.
/// 
/// Implements ILoginNavService to enable Dependency Injection 
/// </summary>
public class LoginNavService : ILoginNavService
{
    private IServiceProvider _serviceProvider; //using ServiceProvider allows use of built-in DependencyInjection
    
    public LoginNavService(IServiceProvider serviceProvider) 
    {
        _serviceProvider = serviceProvider;
    }
    
    /// <summary>
    /// Changes main page to AppShell, granting access to application content
    /// </summary>
    public void RouteOnLogin()
    {
        App.Current.MainPage = _serviceProvider.GetRequiredService<AppShell>(); //changes mainpage to application content
    }

    /// <summary>
    /// Changes main page to login, prompting user to reauthenticate
    /// </summary>
    public void RouteOnLogout()
    {
        App.Current.MainPage = _serviceProvider.GetRequiredService<LoginPage>(); //navigates back to login page
    }
}
