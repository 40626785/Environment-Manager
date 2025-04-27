namespace EnvironmentManager.ViewModels;

/// <summary>
/// Contains data required to add a pin to a mapsui map.
/// </summary>
public class MapPinViewModel
{
    public double Longitude { get; }
    public double Latitude { get; }
    public string Label { get; }

    public MapPinViewModel(double lon, double lat, string label) 
    {
        Longitude = lon;
        Latitude = lat;
        Label = label;
    }
}
