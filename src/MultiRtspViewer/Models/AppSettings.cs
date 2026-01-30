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

        [ObservableProperty]
        private bool lowMemoryMode = false;

        [ObservableProperty]
        private bool ecoMode = true; // Pause streams when minimized

        // Grid Layout Persistence
        [ObservableProperty]
        private int lastGridRows = 2;

        [ObservableProperty]
        private int lastGridColumns = 2;

        [ObservableProperty]
        private int lastClientId = 0; // Track which client's layout this is for

        // Notification Settings
        [ObservableProperty]
        private bool alertsEnabled = false;

        [ObservableProperty]
        private int alertThresholdMinutes = 15;

        [ObservableProperty]
        private AlertProvider provider = AlertProvider.Discord;

        [ObservableProperty]
        private string webhookUrl = "";

        [ObservableProperty]
        private string telegramBotToken = "";

        [ObservableProperty]
        private string telegramChatId = "";

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

    public enum AlertProvider
    {
        Discord,
        Telegram,
        GenericWebhook
    }
}
