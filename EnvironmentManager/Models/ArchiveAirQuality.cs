using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    /// <summary>
    /// Represents the ArchiveAirQuality database context.
    /// </summary>
    public class ArchiveAirQuality
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }
        [Column(TypeName = "time")]
        public TimeSpan? Time { get; set; }
        public double? Nitrogen_dioxide { get; set; }
        public double? Sulphur_dioxide { get; set; }
        public double? PM2_5_particulate_matter { get; set; }
        public double? PM10_particulate_matter { get; set; }
        public int LocationId { get; set; }

    }
}


