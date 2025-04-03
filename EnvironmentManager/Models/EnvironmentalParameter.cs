using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class EnvironmentalParameter
    {
        [Key]
        public int ParameterId { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } // Air/Water/Weather

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Symbol { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; }

        [Required]
        [StringLength(200)]
        public string UnitDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string MeasurementFrequency { get; set; }

        [Required]
        public float SafeLevel { get; set; }

        [StringLength(255)]
        public string ReferenceUrl { get; set; }

        // Navigation property
        public virtual ICollection<SensorReading> SensorReadings { get; set; }

        public EnvironmentalParameter()
        {
            Category = string.Empty;
            Name = string.Empty;
            Symbol = string.Empty;
            Unit = string.Empty;
            UnitDescription = string.Empty;
            MeasurementFrequency = string.Empty;
            ReferenceUrl = string.Empty;
            SensorReadings = new List<SensorReading>();
        }
    }
} 