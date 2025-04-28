using EnvironmentManager.ViewModels;
using EnvironmentManager.Converters;
using EnvironmentManager.Data;

namespace EnvironmentManager.Views
{
    public partial class TrendsPage : ContentPage
    {
        public TrendsPage(ReadingsDbContext dbContext)
        {
            InitializeComponent();
            BindingContext = new TrendsViewModel(dbContext);
        }

                private async void OnCategorySelected(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string category)
            {
                if (BindingContext is TrendsViewModel vm)
                {
                    await vm.LoadDataAsync(category);
                }
            }
        }
        
        //     private void TrendsPage_BindingContextChanged(object sender, EventArgs e)     
        //        {
        //            if (BindingContext is TrendsViewModel vm)          {
        //              vm.TemperatureTrendUpdated += Vm_TemperatureTrendUpdated;
        //          }
        //        }

        //        private void Vm_TemperatureTrendUpdated(object sender, EventArgs e)
        //        {
        //            MainThread.BeginInvokeOnMainThread(() =>
        //            {
        //                TemperatureChartView?.Invalidate(); // Force redrawing the chart
        //            });
        //        }
    }
}