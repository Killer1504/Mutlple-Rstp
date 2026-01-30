using MultiRtspViewer.Models;
using MultiRtspViewer.ViewModels;
using System.Windows;

namespace MultiRtspViewer.Views
{
    public partial class EditCameraDialog : Window
    {
        public EditCameraDialogViewModel ViewModel { get; }

        public EditCameraDialog(CameraModel camera)
        {
            InitializeComponent();
            ViewModel = new EditCameraDialogViewModel(camera);
            DataContext = ViewModel;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ConfirmCommand.Execute(null);
            if (ViewModel.DialogResult)
            {
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelCommand.Execute(null);
            DialogResult = false;
            Close();
        }
    }
}
