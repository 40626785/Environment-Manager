using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class SensorSetting
    {
        [Key]
        public int SettingId { get; set; }

        [Required]
        public int SensorId { get; set; }

        [ForeignKey("SensorId")]
        public virtual Sensor Sensor { get; set; }

        [Required]
        [StringLength(100)]
        public string SettingName { get; set; }

        [Required]
        public string SettingValue { get; set; }

        [Required]
        [StringLength(50)]
        public string DataType { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        public SensorSetting()
        {
            SettingName = string.Empty;
            SettingValue = string.Empty;
            DataType = string.Empty;
            LastUpdated = DateTime.UtcNow;
        }
    }
} 