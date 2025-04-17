using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class ArchiveAirQuality
    {
        [Key]
        public int Id { get; set; } // Auto-generated primary key for EF Core tracking

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? Time { get; set; }

        public double? Nitrogen_dioxide { get; set; }

        public double? Sulphur_dioxide { get; set; }

        public double? PM2_5_particulate_matter { get; set; }

        public double? PM10_particulate_matter { get; set; }
    }
}

