using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

/// <summary>
/// Enables Dependency Inversion Principle, allowing management of login/logout navigation without directly depending on concrete implementation
/// 
/// Implementation of Interface decided in MauiProgram.cs
/// </summary>
public interface ILoginNavService
{
    void RouteOnLogin();
    void RouteOnLogout();
}
