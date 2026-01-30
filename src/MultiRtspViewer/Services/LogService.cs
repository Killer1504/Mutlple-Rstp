using System;
using System.IO;
using System.Text;

namespace MultiRtspViewer.Services
{
    public interface ILogService
    {
        void Log(string message, string level = "INFO");
        void LogError(string message, Exception? ex = null);
        void LogWarning(string message);
    }

    public class LogService : ILogService
    {
        private readonly string _logDirectory;
        private readonly object _lock = new object();

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
            WriteLog($"[{DateTime.Now:HH:mm:ss}] [{level}] {message}");
        }

        public void LogWarning(string message)
        {
            Log(message, "WARN");
        }

        public void LogError(string message, Exception? ex = null)
        {
            var logMessage = message;
            if (ex != null)
            {
                logMessage += $"\nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
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
