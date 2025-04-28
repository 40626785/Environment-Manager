using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    [Table("Readings")]
    public class Reading
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        // Weather readings
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public double? WindSpeed { get; set; }
        public double? WindDirection { get; set; }

        // Air quality readings
        public double? NitrogenDioxide { get; set; } // NO2
        public double? SulphurDioxide { get; set; }  // SO2
        public double? PM25 { get; set; }            // Particulate matter <2.5
        public double? PM10 { get; set; }            // Particulate matter <10

        // Water quality readings
        public double? Nitrate { get; set; }         // NO3
        public double? Nitrite { get; set; }         // NO2
        public double? Phosphate { get; set; }       // PO4
        public int? EColi { get; set; }               // Optional (for future)
        public int? Enterococci { get; set; }         // Optional (for future)


        // Category of the reading (Air, Water, Weather)
        public string? Category { get; set; }
    }
}
