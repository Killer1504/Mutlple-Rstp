using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace MultiRtspViewer.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        public string AppName => "MultiRTSP Viewer";
        public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
        public string Description => "Advanced Multi-Client Surveillance System.\nBuilt with .NET 9, WPF, and LibVLC.";
        public string Copyright => "© 2026 Antigravity";
    }
}
