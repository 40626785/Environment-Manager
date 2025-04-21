using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

/// <summary>
/// Enables Dependency Inversion Principle, allowing ViewModels to retrieve a user without depending on concrete DbContext implementation
/// 
/// Implementation of Interface decided in MauiProgram.cs
/// </summary>
public interface IUserDataStore
{
    User GetUser(string username);
}
