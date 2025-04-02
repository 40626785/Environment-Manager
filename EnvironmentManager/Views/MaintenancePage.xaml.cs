using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class MaintenancePage : ContentPage
{
	public MaintenancePage(MaintenanceViewModel viewModel)
	{
		this.BindingContext = viewModel;
		InitializeComponent();
	}

    public void sliderChanged(object sender, ValueChangedEventArgs args) {
        double value = args.NewValue;
        int convertedValue = Convert.ToInt32(value);
        priorityLabel.Text = $"Priority: {convertedValue}";
    }
}
