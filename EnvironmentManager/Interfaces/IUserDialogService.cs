using System;

namespace EnvironmentManager.Interfaces;

public interface IUserDialogService
{
    Task ShowAlert(string title, string message, string cancel);
    Task<bool> ShowConfirmation(string title, string message, string accept, string cancel);
    Task NavigateBackAsync();
}

