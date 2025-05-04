using System;

namespace EnvironmentManager.Models
{
    public class AirQualityRecord
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public double? Nitrogen_dioxide { get; set; }
        public double? Sulphur_dioxide { get; set; }
        public double? PM2_5_particulate_matter { get; set; }
        public double? PM10_particulate_matter { get; set; }
        public int LocationId { get; set; }

    }
}

