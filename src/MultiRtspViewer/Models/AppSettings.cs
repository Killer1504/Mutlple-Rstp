using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MultiRtspViewer.Models
{
    public partial class AppSettings : ObservableObject
    {
        private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        [ObservableProperty]
        private int networkCaching = 300;

        [ObservableProperty]
        private int avcodecLowres = 2; // 0=full, 1=half, 2=quarter...

        [ObservableProperty]
        private bool disableAudioInGrid = true;

        [ObservableProperty]
        private bool useHardwareAcceleration = true;

        [ObservableProperty]
        private int clockJitter = 0;

        public static AppSettings Load()
        {
            if (File.Exists(SettingsPath))
            {
                try
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                }
                catch { return new AppSettings(); }
            }
            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsPath, json);
            }
            catch { }
        }
    }
}
