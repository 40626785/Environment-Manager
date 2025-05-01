using System;
using EnvironmentManager.Models;

namespace EnvironmentManager.Services
{
    public static class NavigationDataStore
    {
        public static AirQualityRecord SelectedAirQualityRecord { get; set; }
        public static ArchiveAirQuality SelectedArchiveAirQualityRecord { get; set; }


    }
}

