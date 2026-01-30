using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MultiRtspViewer.Models
{
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success
    }

    public partial class NotificationMessage : ObservableObject
    {
        [ObservableProperty]
        private string message = "";

        [ObservableProperty]
        private NotificationType type = NotificationType.Info;

        [ObservableProperty]
        private DateTime timestamp = DateTime.Now;

        [ObservableProperty]
        private bool isVisible = true;

        public NotificationMessage() { }

        public NotificationMessage(string message, NotificationType type = NotificationType.Info)
        {
            Message = message;
            Type = type;
            Timestamp = DateTime.Now;
        }
    }
}
