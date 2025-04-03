using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class SensorReading
    {
        [Key]
        public int ReadingId { get; set; }

        [Required]
        public int SensorId { get; set; }

        [ForeignKey("SensorId")]
        public virtual Sensor Sensor { get; set; }

        [Required]
        public int ParameterId { get; set; }

        [ForeignKey("ParameterId")]
        public virtual EnvironmentalParameter EnvironmentalParameter { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public float Value { get; set; }

        [Required]
        [StringLength(20)]
        public string MeasurementUnit { get; set; }

        [Required]
        public bool IsValid { get; set; }

        public SensorReading()
        {
            MeasurementUnit = string.Empty;
            Timestamp = DateTime.UtcNow;
            IsValid = true;
        }
    }
} 