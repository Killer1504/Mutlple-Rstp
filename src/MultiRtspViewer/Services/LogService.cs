using System;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using MultiRtspViewer.Models;

namespace MultiRtspViewer.Services
{
    public interface ILogService
    {
        ObservableCollection<LogEntry> LogEntries { get; }
        void Log(string message, string level = "INFO");
        void LogError(string message, Exception? ex = null);
        void LogWarning(string message);
        void LogSuccess(string message);
        void LogAi(string message);
    }

    public class LogService : ILogService
    {
        private readonly string _logDirectory;
        private readonly object _lock = new object();
        public ObservableCollection<LogEntry> LogEntries { get; } = new ObservableCollection<LogEntry>();
        private const int MaxEntries = 100;

        public LogService()
        {
            // Store logs in AppData/MultiRtspViewer/logs
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _logDirectory = Path.Combine(appDataPath, "MultiRtspViewer", "logs");

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public void Log(string message, string level = "INFO")
        {
            var color = level switch
            {
                "ERROR" => "#ef4444",   // Red
                "WARN" => "#f59e0b",    // Amber
                "SUCCESS" => "#22c55e", // Green
                "AI" => "#22d3ee",      // Cyan
                _ => "#94a3b8"          // Slate
            };

            var entry = new LogEntry 
            { 
                Message = message, 
                Level = level, 
                Color = color 
            };

            // Add to UI collection on dispatcher thread
            Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
            {
                LogEntries.Insert(0, entry);
                if (LogEntries.Count > MaxEntries)
                {
                    LogEntries.RemoveAt(LogEntries.Count - 1);
                }
            }));

            WriteLog($"[{DateTime.Now:HH:mm:ss}] [{level}] {message}");
        }

        public void LogWarning(string message) => Log(message, "WARN");
        public void LogSuccess(string message) => Log(message, "SUCCESS");
        public void LogAi(string message) => Log(message, "AI");

        public void LogError(string message, Exception? ex = null)
        {
            var logMessage = message;
            if (ex != null)
            {
                logMessage += $"\nException: {ex.Message}";
            }
            Log(logMessage, "ERROR");
        }

        private void WriteLog(string content)
        {
            try
            {
                var fileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
                var filePath = Path.Combine(_logDirectory, fileName);

                lock (_lock)
                {
                    File.AppendAllText(filePath, content + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // Fail silently if logging fails (don't crash the app)
            }
        }
    }
}
