using EnvironmentManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Views
{
    public partial class SensorAnomaliesPage : ContentPage
    {
        public SensorAnomaliesPage(SensorAnomaliesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is SensorAnomaliesViewModel vm)
            {
                   await vm.LoadSensorAnomaliesAsync();
            }
        }
    }
}
