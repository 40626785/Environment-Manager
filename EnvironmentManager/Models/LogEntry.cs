using System;

namespace EnvironmentManager.Models
{
    /// <summary>
    /// Represents the LogEntry database context.
    /// </summary>
    public class LogEntry
    {
        public int LogID { get; set; }
        public DateTime? LogDateTime { get; set; }
        public string LogMessage { get; set; }
    }
}