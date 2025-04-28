namespace EnvironmentManager.Services;

using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using System;
using System.Timers;

/// <summary>
/// Creates and manages a new login session. 
/// 
/// Contains facts about sessions and triggers auto-logout on ttl elapsed
/// </summary>
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

    /// <summary>
    /// Sets facts about session, stores assumed role in local storage and starts session ttl timer.
    /// </summary>
    /// <param name="user">User associated with successful login</param>
    public void NewSession(User user)
    {
        _authenticatedUser = user;
        _expiry = DateTime.Now.AddSeconds(_ttl);
        StoreRole();
        StartTimer();
    }

    /// <summary>
    /// Starts timer that will invoke a method to log a user out following timer completion.
    /// </summary>
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

    /// <summary>
    /// Reroute user to login page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LogoutOnExpire(object sender, ElapsedEventArgs e)
    {
        _mainThread.RunMainThread(() =>
            _loginNavService.RouteOnLogout() //logs out upon timer completion
        );
    }

    /// <summary>
    /// Store user role in local storage to enable Role Based Access Control
    /// </summary>
    private void StoreRole() 
    {
        string roleName = Enum.GetName(typeof(Roles), _authenticatedUser.Role);
        int roleValue = (int)_authenticatedUser.Role;
        
        // Store both the role name and its numeric value to ensure compatibility
        _storageService.SetStringValue("role", roleName);
        _storageService.SetStringValue("roleValue", roleValue.ToString());
    }
}
