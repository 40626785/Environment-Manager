using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        [StringLength(100)]
        public string SiteName { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Elevation { get; set; }

        [Required]
        [StringLength(50)]
        public string SiteType { get; set; }

        [Required]
        [StringLength(50)]
        public string Zone { get; set; }

        [Required]
        [StringLength(100)]
        public string Agglomeration { get; set; }

        [Required]
        [StringLength(100)]
        public string LocalAuthority { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [Required]
        public int UtcOffsetSeconds { get; set; }

        [Required]
        [StringLength(50)]
        public string Timezone { get; set; }

        [Required]
        [StringLength(10)]
        public string TimezoneAbbreviation { get; set; }

        // Navigation property
        public virtual ICollection<Sensor> Sensors { get; set; }

        public Location()
        {
            SiteName = string.Empty;
            SiteType = string.Empty;
            Zone = string.Empty;
            Agglomeration = string.Empty;
            LocalAuthority = string.Empty;
            Country = string.Empty;
            Timezone = string.Empty;
            TimezoneAbbreviation = string.Empty;
            Sensors = new List<Sensor>();
        }
    }
} 