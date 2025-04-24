using System;

namespace EnvironmentManager.Models
{
    public class AirQualityRecord
    {
        public DateTime Date { get; set; }     // Original DateTime from DB
        public string Time { get; set; }

        public double PM25 { get; set; }
        public double PM10 { get; set; }
        public double NO2 { get; set; }
        public double CO { get; set; }

        //  Formatted Date Property for Display
        public string DateFormatted => Date.ToString("yyyy-MM-dd");
    }
}

