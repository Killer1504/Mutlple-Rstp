using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiRtspViewer.Models;
using System;
using System.Text.RegularExpressions;

namespace MultiRtspViewer.ViewModels
{
    public partial class EditCameraDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private string cameraName = "";

        [ObservableProperty]
        private string rtspUrl = "";

        [ObservableProperty]
        private string validationMessage = "";

        [ObservableProperty]
        private bool isValid = false;

        public bool DialogResult { get; private set; }

        public EditCameraDialogViewModel(CameraModel camera)
        {
            CameraName = camera.Name;
            RtspUrl = camera.RtspUrl;
            ValidateUrl();
        }

        partial void OnRtspUrlChanged(string value)
        {
            ValidateUrl();
        }

        partial void OnCameraNameChanged(string value)
        {
            ValidateUrl();
        }

        private void ValidateUrl()
        {
            if (string.IsNullOrWhiteSpace(CameraName))
            {
                ValidationMessage = "Camera name is required";
                IsValid = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(RtspUrl))
            {
                ValidationMessage = "RTSP URL is required";
                IsValid = false;
                return;
            }

            // Validate RTSP URL format
            var rtspPattern = @"^rtsp://.*";
            if (!Regex.IsMatch(RtspUrl.Trim(), rtspPattern, RegexOptions.IgnoreCase))
            {
                ValidationMessage = "URL must start with rtsp://";
                IsValid = false;
                return;
            }

            // Try to parse as URI
            if (!Uri.TryCreate(RtspUrl.Trim(), UriKind.Absolute, out var uri))
            {
                ValidationMessage = "Invalid URL format";
                IsValid = false;
                return;
            }

            ValidationMessage = "✓ Valid RTSP URL";
            IsValid = true;
        }

        [RelayCommand]
        private void UseExample(string exampleUrl)
        {
            RtspUrl = exampleUrl;
        }

        [RelayCommand(CanExecute = nameof(IsValid))]
        private void Confirm()
        {
            DialogResult = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
        }
    }
}
