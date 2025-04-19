namespace EnvironmentManager.Services;

using EnvironmentManager.Interfaces;
using EnvironmentManager.Views;

public class LoginNavService : ILoginNavService
{
    private IServiceProvider _serviceProvider; //using ServiceProvider allows use of built-in DependencyInjection
    
    public LoginNavService(IServiceProvider serviceProvider) 
    {
        _serviceProvider = serviceProvider;
    }
    
    public void RouteOnLogin()
    {
        App.Current.MainPage = _serviceProvider.GetRequiredService<AppShell>(); //changes mainpage to application content
    }

    public void RouteOnLogout()
    {
        App.Current.MainPage = _serviceProvider.GetRequiredService<LoginPage>(); //navigates back to login page
    }
}
