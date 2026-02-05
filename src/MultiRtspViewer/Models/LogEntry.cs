using System;

namespace MultiRtspViewer.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = "INFO"; // INFO, WARN, ERROR, AI, SUCCESS
        public string Color { get; set; } = "#94a3b8"; // Default Slate-400
        
        public string FormattedTime => Timestamp.ToString("HH:mm:ss");
    }
}
