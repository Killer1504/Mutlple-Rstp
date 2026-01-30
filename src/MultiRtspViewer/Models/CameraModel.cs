using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MultiRtspViewer.Models
{
    public enum ConnectionStatus
    {
        Offline,
        Connecting,
        Connected,
        Reconnecting,
        Error
    }

    public partial class CameraModel : ObservableObject
    {
        [ObservableProperty]
        private string id = Guid.NewGuid().ToString();

        [ObservableProperty]
        private string name = "New Camera";

        [ObservableProperty]
        private string rtspUrl = "";

        // Legacy string status for backward compatibility
        [ObservableProperty]
        private string status = "Offline";

        // New enum-based connection status
        [ObservableProperty]
        private ConnectionStatus connectionStatus = ConnectionStatus.Offline;

        // Health monitoring
        [ObservableProperty]
        private DateTime? lastHeartbeat;

        [ObservableProperty]
        private int reconnectAttempts = 0;

        public CameraModel() { }
    }
}
