using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class Sensor
    {
        [Key]
        public int SensorId { get; set; }

        // Foreign Keys - Assuming Location and Account models will be created later
        // public int LocationId { get; set; }
        // public virtual Location Location { get; set; } // Navigation property
        // public int AccountId { get; set; } // Assuming an Account model exists or will be created
        // public virtual Account Account { get; set; } // Navigation property

        [Required]
        [StringLength(100)]
        public string SensorName { get; set; }

        [StringLength(100)]
        public string Model { get; set; }

        [StringLength(100)]
        public string Manufacturer { get; set; }

        [StringLength(50)]
        public string SensorType { get; set; } // e.g., Temperature, Humidity, CO2

        public DateTime InstallationDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastCalibration { get; set; } // Nullable DateTime

        [StringLength(50)]
        public string FirmwareVersion { get; set; }

        [StringLength(200)]
        public string DataSource { get; set; } // e.g., API endpoint, MQTT topic

        [Url]
        [StringLength(255)]
        public string SensorUrl { get; set; } // URL for accessing sensor data/interface

        public DateTime? LastHeartbeat { get; set; } // Nullable DateTime

        [StringLength(50)]
        public string ConnectivityStatus { get; set; } // e.g., "Online", "Offline", "Degraded"

        public float? BatteryLevelPercentage { get; set; } // Nullable float

        public int? SignalStrengthDbm { get; set; } // Nullable int

        // Navigation properties for related entities (based on ERD)
        // public virtual ICollection<SensorReading> SensorReadings { get; set; }
        // public virtual ICollection<SensorSetting> SensorSettings { get; set; }
        // public virtual ICollection<CalibrationRecord> CalibrationRecords { get; set; }
        // public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } // Note: MaintenanceRecord model already exists partially
        // public virtual ICollection<SensorStatusLog> SensorStatusLogs { get; set; }

        public Sensor()
        {
            // Initialize collections if needed
            // SensorReadings = new HashSet<SensorReading>();
            // SensorSettings = new HashSet<SensorSetting>();
            // CalibrationRecords = new HashSet<CalibrationRecord>();
            // MaintenanceRecords = new HashSet<MaintenanceRecord>();
            // SensorStatusLogs = new HashSet<SensorStatusLog>();
        }
    }
}
