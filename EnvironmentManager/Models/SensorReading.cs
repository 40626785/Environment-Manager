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
        public DateTime Timestamp { get; set; }

        [Required]
        public string ParameterName { get; set; } // e.g., "Temperature", "Humidity"

        [Required]
        public double Value { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } // e.g., "°C", "%RH"

        public string? Status { get; set; } // e.g., "Normal", "Warning", "Error"

        public string? Notes { get; set; }

        public SensorReading()
        {
            ParameterName = string.Empty;
            Unit = string.Empty;
            Timestamp = DateTime.UtcNow;
        }
    }
} 