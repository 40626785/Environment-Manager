using System;

namespace EnvironmentManager.Models
{
    public class LogEntry
    {
        public int LogID { get; set; }
        public DateTime? LogDateTime { get; set; }
        public string LogMessage { get; set; }
    }
}
