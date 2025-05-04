using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models
{
    public class User
    {
        [Key]
        [StringLength(20)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int Role { get; set; }
    }
}
