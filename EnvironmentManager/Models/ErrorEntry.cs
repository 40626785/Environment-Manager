using System;

namespace EnvironmentManager.Models
{
    public class ErrorEntry
    {
        public int ErrorID { get; set; }
        public DateTime? ErrorDateTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}
