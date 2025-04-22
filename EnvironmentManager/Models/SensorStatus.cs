using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class SensorStatus
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        public int SensorId { get; set; }

        [ForeignKey("SensorId")]
        public virtual Sensor Sensor { get; set; }

        [Required]
        [StringLength(50)]
        public string ConnectivityStatus { get; set; }

        [Required]
        public DateTime StatusTimestamp { get; set; }

        public float? BatteryLevelPercentage { get; set; }
        
        public int? ErrorCount { get; set; }
        
        public int? WarningCount { get; set; }

        public SensorStatus()
        {
            // Initialize default values
            ConnectivityStatus = "Offline";
            StatusTimestamp = DateTime.Now;
            ErrorCount = 0;
            WarningCount = 0;
        }
    }
} 
