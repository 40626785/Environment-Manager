namespace EnvironmentManager.Services;

using EnvironmentManager.Interfaces;
using EnvironmentManager.Views;

/// <summary>
/// Allows running operations on Main Thread, required by UI actions
/// 
/// Implements IRunOnMainThread to enable Dependency Injection 
/// </summary>
public class RunOnMainThread : IRunOnMainThread 
{   
    /// <summary>
    /// Runs passed action on the MainThread
    /// </summary>
    /// <param name="action"></param>
    /// <example>
    /// The following example demonstrates how to pass an action to be run on Main Thread
    /// <code>
    /// _mainThread = new RunOnMainThread();
    /// _mainThread.RunMainThread(() => actionToRun())
    /// </code>
    /// </example>
    public void RunMainThread(Action action)
    {
        MainThread.BeginInvokeOnMainThread(action);
    }
}
