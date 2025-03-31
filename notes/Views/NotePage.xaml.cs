using notes.ViewModels;

namespace notes.Views;

public partial class NotePage : ContentPage
{
    public NotePage(NoteViewModel viewModel)
    {
        this.BindingContext = viewModel;
        InitializeComponent();
    }

}