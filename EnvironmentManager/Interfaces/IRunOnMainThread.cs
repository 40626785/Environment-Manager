using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

/// <summary>
/// Enables Dependency Inversion Principle, allowing running on main thread without directly depending on concrete implementation
/// 
/// Implementation of Interface decided in MauiProgram.cs
/// </summary>
public interface IRunOnMainThread
{
    public void RunMainThread(Action action);
}
