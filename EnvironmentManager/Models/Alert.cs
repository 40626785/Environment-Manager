using System;

namespace EnvironmentManager.Models
{
    public class Alert
    {
        public int AlertId { get; set; }
        public int LocationId { get; set; }
        public DateTime Date_Time { get; set; }
        public string Parameter { get; set; }
        public double? Value { get; set; }
        public double? Deviation { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsResolved { get; set; }
    }
}

