using Microsoft.EntityFrameworkCore;
using notes.Models;

namespace notes.Data;
public class NotesDbContext : DbContext
{

    public NotesDbContext()
    { }
    public NotesDbContext(DbContextOptions options) : base(options)
    { }

    public DbSet<Note> Notes { get; set; }

}
