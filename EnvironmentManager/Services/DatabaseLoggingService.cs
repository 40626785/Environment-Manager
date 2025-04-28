using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;

using System.Threading.Tasks;
using System;

namespace EnvironmentManager.Services;

public class DatabaseLoggingService
{
    private readonly DatabaseAdminDbContext _context;

    public DatabaseLoggingService(DatabaseAdminDbContext context)
    {
        _context = context;
    }

    public async Task LogErrorAsync(string errorMessage)
    {
        var sql = "EXEC LogError @p0";
        await _context.Database.ExecuteSqlRawAsync(sql, errorMessage);
    }

    public async Task LogMessageAsync(string message)
    {
        var sql = "EXEC LogMessage @p0";
        await _context.Database.ExecuteSqlRawAsync(sql, message);
    }
}
