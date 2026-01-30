using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MultiRtspViewer.Models
{
    public partial class CameraModel : ObservableObject
    {
        [ObservableProperty]
        private string id = Guid.NewGuid().ToString();

        [ObservableProperty]
        private string name = "New Camera";

        [ObservableProperty]
        private string rtspUrl = "";

        // Status: "Connecting", "Online", "Offline", "Error"
        [ObservableProperty]
        private string status = "Offline";

        public CameraModel() { }
    }
}
