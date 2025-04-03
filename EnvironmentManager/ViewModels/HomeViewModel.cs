using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
// using notes.Views; // Removed unnecessary using

namespace EnvironmentManager.ViewModels; // Corrected namespace

public partial class HomeViewModel : ObservableObject
{
    public HomeViewModel()
    {
        // Constructor remains empty for now
    }

    // Removed RelayCommands for NavigateToNotes, NavigateToAllNotes, and NavigateToAbout
    // as they relate to removed functionality or pages outside the scope
    // of homepage/sensor page.
}
