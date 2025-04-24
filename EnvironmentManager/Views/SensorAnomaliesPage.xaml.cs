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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is SensorAnomaliesViewModel vm)
            {
                   vm.GetType().GetMethod("LoadSensorAnomalies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(vm, null);
            }
        }
    }
}
