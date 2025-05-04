using System;
using EnvironmentManager.Models;
using LocationModel = EnvironmentManager.Models.Location;


namespace EnvironmentManager.Services
{
    public static class NavigationDataStore
    {
        public static AirQualityRecord SelectedAirQualityRecord { get; set; }
        public static ArchiveAirQuality SelectedArchiveAirQualityRecord { get; set; }

        public static EnvironmentManager.Models.Location? SelectedLocationRecord { get; set; }
        public static User? SelectedUserRecord { get; set; }



    }
}

