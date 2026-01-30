using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MultiRtspViewer.Models;
using MultiRtspViewer.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MultiRtspViewer.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly LibVLC _libVLC;
        private readonly ConfigService _configService;

        [ObservableProperty]
        private ObservableCollection<CameraViewModel> cameras = new();

        [ObservableProperty]
        private int rows = 2;

        [ObservableProperty]
        private int columns = 2;

        [ObservableProperty]
        private bool isManagementPanelVisible = false;

        public bool HasNoCameras => Cameras.Count == 0;


        public MainViewModel()
        {
            _configService = new ConfigService();
            
            // Enable hardware decoding
            _libVLC = new LibVLC("--avcodec-hw=d3d11va", "--network-caching=300");

            LoadCameras();
        }

        private void LoadCameras()
        {
            var savedCameras = _configService.LoadCameras();
            foreach (var model in savedCameras)
            {
                var cameraVm = new CameraViewModel(model, _libVLC);
                Cameras.Add(cameraVm);
            }
            UpdateLayout();
        }

        // Call this after UI is loaded
        public void StartAllCameras()
        {
            foreach (var camera in Cameras)
            {
                camera.Play();
            }
        }

        [RelayCommand]
        public void AddCamera()
        {
            var dialog = new Views.AddCameraDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                var vm = dialog.ViewModel;
                var newCamera = new CameraModel 
                { 
                    Name = vm.CameraName.Trim(),
                    RtspUrl = vm.RtspUrl.Trim()
                };
                
                var cameraVm = new CameraViewModel(newCamera, _libVLC);
                Cameras.Add(cameraVm);
                
                // Auto-play the newly added camera
                cameraVm.Play();
                
                SaveState();
                UpdateLayout();
                OnPropertyChanged(nameof(HasNoCameras));
            }
        }

        private void SaveState()
        {
            var models = Cameras.Select(vm => vm.Model).ToList();
            _configService.SaveCameras(models);
        }

        [RelayCommand]
        public void SetLayout(string layoutType)
        {
            switch (layoutType)
            {
                case "1x1": Rows = 1; Columns = 1; break;
                case "2x2": Rows = 2; Columns = 2; break;
                case "3x3": Rows = 3; Columns = 3; break;
                case "4x4": Rows = 4; Columns = 4; break;
                case "Auto": UpdateLayout(); break;
            }
        }

        private void UpdateLayout()
        {
            int count = Cameras.Count;
            if (count == 0) return;

            int dim = (int)Math.Ceiling(Math.Sqrt(count));
            Rows = dim;
            Columns = dim;
        }

        [RelayCommand]
        public void ToggleManagementPanel()
        {
            IsManagementPanelVisible = !IsManagementPanelVisible;
        }

        [RelayCommand]
        public void EditCamera(CameraViewModel cameraVm)
        {
            var dialog = new Views.EditCameraDialog(cameraVm.Model)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                var vm = dialog.ViewModel;
                cameraVm.Model.Name = vm.CameraName.Trim();
                cameraVm.Model.RtspUrl = vm.RtspUrl.Trim();
                
                // Restart stream with new URL
                cameraVm.Stop();
                cameraVm.Play();
                
                SaveState();
            }
        }

        [RelayCommand]
        public void DeleteCamera(CameraViewModel cameraVm)
        {
            var result = System.Windows.MessageBox.Show(
                $"Are you sure you want to delete '{cameraVm.Model.Name}'?",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                cameraVm.Dispose();
                Cameras.Remove(cameraVm);
                SaveState();
                UpdateLayout();
                OnPropertyChanged(nameof(HasNoCameras));
            }
        }

        [RelayCommand]
        public void MoveCameraUp(CameraViewModel cameraVm)
        {
            int index = Cameras.IndexOf(cameraVm);
            if (index > 0)
            {
                Cameras.Move(index, index - 1);
                SaveState();
            }
        }

        [RelayCommand]
        public void MoveCameraDown(CameraViewModel cameraVm)
        {
            int index = Cameras.IndexOf(cameraVm);
            if (index < Cameras.Count - 1)
            {
                Cameras.Move(index, index + 1);
                SaveState();
            }
        }


        public void Dispose()
        {
            SaveState(); // Save on exit
            foreach (var cam in Cameras)
            {
                cam.Dispose();
            }
            _libVLC?.Dispose();
        }
    }
}
