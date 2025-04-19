namespace EnvironmentManager.Services;

using EnvironmentManager.Interfaces;
using EnvironmentManager.Views;

public class RunOnMainThread : IRunOnMainThread 
{   
    public void RunMainThread(Action action)
    {
        MainThread.BeginInvokeOnMainThread(action);
    }
}
