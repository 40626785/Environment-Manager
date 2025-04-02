using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;

namespace EnvironmentManager.Data;
public class NotesDbContext : DbContext
{

    public NotesDbContext()
    { }
    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    { }

    public DbSet<Note> Notes { get; set; }

}
