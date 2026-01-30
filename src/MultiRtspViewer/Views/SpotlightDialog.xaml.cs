using System.Windows;
using MultiRtspViewer.ViewModels;
using LibVLCSharp.Shared;

namespace MultiRtspViewer.Views
{
    public partial class SpotlightDialog : Window
    {
        private CameraViewModel _viewModel;

        public SpotlightDialog(CameraViewModel sourceViewModel, LibVLC libVLC, Models.AppSettings settings)
        {
            InitializeComponent();
            
            // Create a NEW ViewModel for this window to avoid affecting the grid
            var modelCopy = new Models.CameraModel
            {
                Id = sourceViewModel.Model.Id,
                Name = sourceViewModel.Model.Name,
                RtspUrl = sourceViewModel.Model.RtspUrl
            };

            _viewModel = new CameraViewModel(modelCopy, libVLC, settings);
            _viewModel.IsFullQuality = true; // Set High Quality
            
            CameraNameText.Text = modelCopy.Name;
            VideoPlayer.MediaPlayer = _viewModel.MediaPlayer;
            
            _viewModel.Play();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Stop();
            _viewModel.Dispose();
            Close();
        }
    }
}
