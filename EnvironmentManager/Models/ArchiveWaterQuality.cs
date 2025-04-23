using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class ArchiveWaterQuality
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? Time { get; set; }

        public double? Nitrate_mg_l_1 { get; set; }

        public double? Nitrite_less_thank_mg_l_1 { get; set; }

        public double? Phosphate_mg_l_1 { get; set; }

        public double? EC_cfu_100ml { get; set; }
    }
}
