using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class Sensor
    {
        [Key]
        public int SensorId { get; set; }

        [Required]
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        [Required]
        [StringLength(100)]
        public string SensorName { get; set; }

        [StringLength(100)]
        public string Model { get; set; }

        [StringLength(100)]
        public string Manufacturer { get; set; }

        [StringLength(50)]
        public string SensorType { get; set; } // e.g., Temperature, Humidity, CO2

        [Required]
        public DateTime InstallationDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [StringLength(50)]
        public string FirmwareVersion { get; set; }

        [StringLength(200)]
        public string DataSource { get; set; } // e.g., API endpoint, MQTT topic

        [Url]
        [StringLength(255)]
        public string SensorUrl { get; set; } // URL for accessing sensor data/interface

        [StringLength(50)]
        public string ConnectivityStatus { get; set; } // e.g., "Online", "Offline", "Degraded"

        public float? BatteryLevelPercentage { get; set; } // Nullable float

        // Navigation properties for related entities

        public Sensor()
        {
            // Initialize default values if needed
            SensorName = string.Empty;
            Model = string.Empty;
            Manufacturer = string.Empty;
            SensorType = string.Empty;
            FirmwareVersion = string.Empty;
            DataSource = string.Empty;
            SensorUrl = string.Empty;
            ConnectivityStatus = string.Empty;
            InstallationDate = DateTime.Now;
            IsActive = true;

            // Initialize collections
        }
    }
}
