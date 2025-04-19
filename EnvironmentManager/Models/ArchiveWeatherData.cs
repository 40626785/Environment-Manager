using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class ArchiveWeatherData
    {
        [Key]
        [Column(TypeName = "datetime")]
        public DateTime Date_Time { get; set; }

        public double Temperature_2m { get; set; }

        public double Relative_humidity_2m { get; set; }

        public double Wind_speed_10m { get; set; }

        public double Wind_direction_10m { get; set; }
    }
}
