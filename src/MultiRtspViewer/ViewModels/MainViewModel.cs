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
        private readonly CameraService _cameraService;

        [ObservableProperty]
        private ObservableCollection<CameraViewModel> cameras = new();

        [ObservableProperty]
        private int rows = 2;

        [ObservableProperty]
        private int columns = 2;

        [ObservableProperty]
        private bool isManagementPanelVisible = false;

        [ObservableProperty]
        private bool isSidebarVisible = true;

        [RelayCommand]
        public void ToggleSidebar()
        {
            IsSidebarVisible = !IsSidebarVisible;
        }

        public bool HasNoCameras => Cameras.Count == 0;

        public SidebarViewModel Sidebar { get; }

        public MainViewModel()
        {
            Sidebar = new SidebarViewModel();
            Sidebar.PropertyChanged += Sidebar_PropertyChanged;

            _cameraService = new CameraService();
            
            // Enable hardware decoding
            _libVLC = new LibVLC("--avcodec-hw=d3d11va", "--network-caching=300");

            if (Sidebar.SelectedClient != null)
            {
                LoadCameras(autoPlay: false); // Defer play until Window Loaded
            }
        }

        private void Sidebar_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SidebarViewModel.SelectedClient))
            {
                LoadCameras(autoPlay: true);
            }
        }

        private void LoadCameras(bool autoPlay = true)
        {
            // Clean up existing players
            foreach (var cam in Cameras) { cam.Dispose(); }
            Cameras.Clear();

            if (Sidebar.SelectedClient == null) 
            {
                UpdateLayout();
                OnPropertyChanged(nameof(HasNoCameras));
                return;
            }

            var dbCameras = _cameraService.GetCamerasByClient(Sidebar.SelectedClient.Id);
            
            foreach (var dbCam in dbCameras)
            {
                var model = new CameraModel
                {
                    Id = dbCam.Id.ToString(), // Map DB ID to Model ID
                    Name = dbCam.Name,
                    RtspUrl = dbCam.RtspUrl,
                    Status = "Offline"
                };

                var cameraVm = new CameraViewModel(model, _libVLC);
                Cameras.Add(cameraVm);
            }
            
            // Only play if requested (runtime switch), otherwise wait for Window_Loaded
            if (autoPlay)
            {
                foreach(var cam in Cameras) { cam.Play(); }
            }

            UpdateLayout();
            OnPropertyChanged(nameof(HasNoCameras));
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
            if (Sidebar.SelectedClient == null)
            {
                System.Windows.MessageBox.Show("Please select a Client first.", "No Client Selected");
                return;
            }

            var dialog = new Views.AddCameraDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                var vm = dialog.ViewModel;
                
                // IDB Insert
                var dbCam = _cameraService.AddCamera(Sidebar.SelectedClient.Id, vm.CameraName.Trim(), vm.RtspUrl.Trim());

                var model = new CameraModel 
                { 
                    Id = dbCam.Id.ToString(),
                    Name = dbCam.Name,
                    RtspUrl = dbCam.RtspUrl
                };
                
                var cameraVm = new CameraViewModel(model, _libVLC);
                Cameras.Add(cameraVm);
                
                // Auto-play the newly added camera
                cameraVm.Play();
                
                // Update Client Camera Count UI
                Sidebar.SelectedClient.CameraCount++;

                UpdateLayout();
                OnPropertyChanged(nameof(HasNoCameras));
            }
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
                
                // DB Update
                if (int.TryParse(cameraVm.Model.Id, out int camId))
                {
                    var dbCam = new Models.Database.Camera 
                    {
                        Id = camId,
                        ClientId = Sidebar.SelectedClient?.Id ?? 0,
                        Name = cameraVm.Model.Name,
                        RtspUrl = cameraVm.Model.RtspUrl,
                        Position = 0 // Should preserve existing position properly but simplifying for now
                    };
                    _cameraService.UpdateCamera(dbCam);
                }

                // Restart stream with new URL
                cameraVm.Stop();
                cameraVm.Play();
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
                // DB Delete
                if (int.TryParse(cameraVm.Model.Id, out int camId))
                {
                    _cameraService.DeleteCamera(camId);
                }

                cameraVm.Dispose();
                Cameras.Remove(cameraVm);

                // Update Client Count
                if (Sidebar.SelectedClient != null) Sidebar.SelectedClient.CameraCount--;

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
                SavePositions();
            }
        }

        [RelayCommand]
        public void MoveCameraDown(CameraViewModel cameraVm)
        {
            int index = Cameras.IndexOf(cameraVm);
            if (index < Cameras.Count - 1)
            {
                Cameras.Move(index, index + 1);
                SavePositions();
            }
        }

        private void SavePositions()
        {
            var dbCameras = new System.Collections.Generic.List<Models.Database.Camera>();
            for(int i = 0; i < Cameras.Count; i++)
            {
                if (int.TryParse(Cameras[i].Model.Id, out int camId))
                {
                    dbCameras.Add(new Models.Database.Camera { Id = camId, Position = i });
                }
            }
            _cameraService.UpdateCameraPositions(dbCameras);
        }


        public void Dispose()
        {
            foreach (var cam in Cameras)
            {
                cam.Dispose();
            }
            _libVLC?.Dispose();
        }
    }
}
