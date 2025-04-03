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
        public string Value { get; set; }

        [StringLength(50)]
        public string? DataType { get; set; } // e.g., "string", "number", "boolean"

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastModified { get; set; }

        public string? ModifiedBy { get; set; }

        public SensorSetting()
        {
            SettingName = string.Empty;
            Value = string.Empty;
            IsActive = true;
            LastModified = DateTime.UtcNow;
        }
    }
} 