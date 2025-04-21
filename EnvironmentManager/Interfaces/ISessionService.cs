using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;
/// <summary>
/// Enables Dependency Inversion Principle, allowing management of sessions without directly depending on session class
/// 
/// Implementation of Interface decided in MauiProgram.cs
/// </summary>
public interface ISessionService
{
   User? AuthenticatedUser { get; }
   DateTime? Expiry { get; }
    
   void NewSession(User user);
}
