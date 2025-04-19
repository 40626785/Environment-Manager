using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

//Allows changing MainPage without handling within ViewModel and breaking MVVM structure
public interface ILoginNavService
{
    void RouteOnLogin();
    void RouteOnLogout();
}
