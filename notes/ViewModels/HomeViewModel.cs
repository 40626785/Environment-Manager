using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using notes.Views;

namespace notes.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    public HomeViewModel()
    {
    }

    [RelayCommand]
    async Task NavigateToNotes()
    {
        await Application.Current.MainPage.Navigation.PushAsync(new NotePage());
    }

    [RelayCommand]
    async Task NavigateToAllNotes()
    {
        await Application.Current.MainPage.Navigation.PushAsync(new AllNotesPage());
    }

    [RelayCommand]
    async Task NavigateToAbout()
    {
        await Application.Current.MainPage.Navigation.PushAsync(new AboutPage());
    }
} 