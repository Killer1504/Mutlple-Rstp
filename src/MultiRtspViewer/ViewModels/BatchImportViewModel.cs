using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiRtspViewer.ViewModels
{
    public partial class BatchImportViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ValidationSummary))]
        private string rawText = "";

        public string ValidationSummary
        {
            get
            {
                var parsed = GetParsedCameras();
                int total = RawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                int valid = parsed.Count;
                return $"{valid} valid cameras detected out of {total} lines.";
            }
        }

        public List<(string name, string url)> GetParsedCameras()
        {
            var results = new List<(string name, string url)>();
            var lines = RawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int autoNameCounter = 1;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                string name;
                string url;

                if (trimmed.Contains("|"))
                {
                    var parts = trimmed.Split(new[] { '|' }, 2);
                    name = parts[0].Trim();
                    url = parts[1].Trim();
                }
                else
                {
                    name = $"Camera {autoNameCounter++}";
                    url = trimmed;
                }

                // Basic URL validation
                if (url.StartsWith("rtsp://", StringComparison.OrdinalIgnoreCase))
                {
                    results.Add((name, url));
                }
            }

            return results;
        }
    }
}
