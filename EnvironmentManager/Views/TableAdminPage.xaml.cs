using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class TableAdminPage : ContentPage, IQueryAttributable
{
	public TableAdminPage(AirQualityAdminViewModel airVm, ErrorTableAdminViewModel errorVm)
	{
		InitializeComponent();
		_airVm = airVm;
		_errorVm = errorVm;
	}

	private readonly AirQualityAdminViewModel _airVm;
	private readonly ErrorTableAdminViewModel _errorVm;

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue("table", out var tableName))
		{
			switch (tableName.ToString())
			{
				case "Air_Quality":
					_airVm.LoadData();
					BindingContext = _airVm;
					break;
				case "ErrorTable":
					_errorVm.LoadData();
					BindingContext = _errorVm;
					break;
			}
		}
	}
}
