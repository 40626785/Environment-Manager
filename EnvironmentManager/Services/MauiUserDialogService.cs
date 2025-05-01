using System;
using System.Threading.Tasks;
using EnvironmentManager.Interfaces;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Services;

public class MauiUserDialogService : IUserDialogService
{
    public Task ShowAlert(string title, string message, string cancel)
    {
        return Application.Current.MainPage.DisplayAlert(title, message, cancel);
    }

    public Task NavigateBackAsync()
    {
        return Shell.Current.GoToAsync("..");
    }
}
