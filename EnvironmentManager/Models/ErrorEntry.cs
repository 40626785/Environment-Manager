using System;

namespace EnvironmentManager.Models
{
    /// <summary>
    /// Represents the ErrorEntry database context.
    /// </summary>
    public class ErrorEntry
    {
        public int ErrorID { get; set; }
        public DateTime? ErrorDateTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}