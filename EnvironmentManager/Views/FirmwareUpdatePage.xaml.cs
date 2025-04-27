using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
    public partial class FirmwareUpdatePage : ContentPage
    {
        public FirmwareUpdatePage(FirmwareUpdateViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is FirmwareUpdateViewModel vm)
            {
                vm.LoadSensorsCommand?.Execute(null);
            }
        }
    }
}
