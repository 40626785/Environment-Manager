using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
    /// <summary>
    /// Code-behind for AnomalyPage, initializes ViewModel binding.
    /// </summary>
    public partial class AnomalyPage : ContentPage
    {
        public AnomalyPage(AnomalyDetectionViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is AnomalyDetectionViewModel vm)
            {
                await vm.LoadAnomaliesAsync(); // Load anomalies when the page appears
            }
        }
    }
}