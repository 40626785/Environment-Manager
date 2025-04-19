namespace EnvironmentManager.Services;

using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Diagnostics;
using System.Timers;

public class SessionService : ISessionService
{
    private int _ttl = 3600; //duration of a login session
    private User _authenticatedUser;
    private DateTime _expiry;
    private ILoginNavService _loginNavService;
    private IRunOnMainThread _mainThread;
    private ILocalStorageService _storageService;

    private Timer _timer;

    public User? AuthenticatedUser => _authenticatedUser;
    public DateTime? Expiry => _expiry;

    public SessionService(ILoginNavService loginNavService, IRunOnMainThread mainThread, ILocalStorageService storageService)
    {
        _loginNavService = loginNavService;
        _mainThread = mainThread;
        _storageService = storageService;
    }
    
    public void NewSession(User user){
        _authenticatedUser = user;
        _expiry = DateTime.Now.AddSeconds(_ttl);
        StoreRole();
        StartTimer();
    }

    private void StartTimer() 
    {
        _timer?.Stop(); //clear any old timer created from previous session
        _timer?.Dispose();
        TimeSpan sessionDuration = TimeSpan.FromSeconds(_ttl);
        _timer = new Timer(sessionDuration.TotalMilliseconds);
        _timer.Elapsed += LogoutOnExpire; //bind logout function to run on timer completion
        _timer.AutoReset = false; //timer does not rerun once complete
        _timer.Start();
    }

    private void LogoutOnExpire(object sender, ElapsedEventArgs e)
    {
        _mainThread.RunMainThread(() =>
            _loginNavService.RouteOnLogout() //logs out upon timer completion
        );
    }

    //adds role to local storage to enable role based access control
    private void StoreRole() 
    {
        _storageService.SetStringValue("role", _authenticatedUser.Role.ToString());
    }
}
