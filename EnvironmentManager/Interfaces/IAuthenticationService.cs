using System;
using EnvironmentManager.Models;
using EnvironmentManager.Services;

namespace EnvironmentManager.Interfaces;

/// <summary>
/// Enables Dependency Inversion Principle, allowing management of authentication without directly depending on session class
/// 
/// Implementation of Interface decided in MauiProgram.cs
/// </summary>
public interface IAuthenticationService
{
    bool Authenticated { get; }
    User AuthenticatedUser { get; }
    
    void Authenticate(string username, string password);
}
