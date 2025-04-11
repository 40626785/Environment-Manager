using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnvironmentManager.Models;

[Table("maintenance")]
[PrimaryKey(nameof(Id))]
public class Maintenance
{
    public int Id { get; set; }
    //[Required]
    //public Sensor sensor { get; set; }
    [Required]
    public DateTime DueDate { get; set; }
    [Required]
    public bool Overdue { get; set; }
    [Required]
    public int Priority { get; set; }
    [Required]
    public string Description { get; set; }
}
