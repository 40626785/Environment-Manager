using Mapsui.UI.Maui;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui;
using Mapsui.Nts;
using Mapsui.Projections;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Models;

namespace EnvironmentManager.Views;

public partial class ThresholdMapPage : ContentPage
{
	public ThresholdMapPage(ThresholdMapViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
 
    private void AddBreaches(double longitude, double latitude, string label, MapControl mapControl)
    {
        var coordinate = SphericalMercator.FromLonLat(longitude, latitude);
        NetTopologySuite.Geometries.Point point = new NetTopologySuite.Geometries.Point(coordinate.x, coordinate.y);
        var feature = new GeometryFeature { Geometry = point };
        feature.Styles.Add(new LabelStyle
        {
            Text = $"{label}",
            ForeColor = Mapsui.Styles.Color.Black,
            BackColor = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red),
            Offset = new Offset(0, 50),
            Font = new Mapsui.Styles.Font { Size = 14 }
        });
        var layer = new MemoryLayer
        {
            Name = "pointLayer",
            Features = new List<IFeature> { feature }
        };
        mapControl.Map.Layers.Add(layer);
        mapControl.Refresh();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var mapControl = new MapControl{
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand
        };
        mapControl.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        
        grid.Children.Add(mapControl);

        var viewModel = (ThresholdMapViewModel)BindingContext;

        List<MapPinViewModel> pins = viewModel.GetSensorBreachPins();

        foreach (MapPinViewModel pin in pins)
        {
            AddBreaches(pin.Longitude, pin.Latitude, pin.Label, mapControl);
        }
    }
}
