using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.Services
{
    public class DatabaseLoggingService : ILoggingService
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
}
