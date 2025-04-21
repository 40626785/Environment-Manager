using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Interfaces;

//Enables Dependency Inversion Principle, allowing running on main thread without using concrete class.
public interface IRunOnMainThread
{
    public void RunMainThread(Action action);
}
