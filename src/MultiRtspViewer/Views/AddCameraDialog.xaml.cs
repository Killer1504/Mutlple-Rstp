using MultiRtspViewer.ViewModels;
using System.Windows;

namespace MultiRtspViewer.Views
{
    public partial class AddCameraDialog : Window
    {
        public AddCameraDialogViewModel ViewModel { get; }

        public AddCameraDialog()
        {
            InitializeComponent();
            ViewModel = (AddCameraDialogViewModel)DataContext;
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
