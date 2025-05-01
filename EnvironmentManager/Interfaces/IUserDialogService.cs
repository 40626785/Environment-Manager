using System;

namespace EnvironmentManager.Interfaces;

public interface IUserDialogService
{
    Task ShowAlert(string title, string message, string cancel);
    Task NavigateBackAsync();
}

