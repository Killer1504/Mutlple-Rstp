using System.Windows;
using MultiRtspViewer.ViewModels;

namespace MultiRtspViewer.Views
{
    public partial class AddClientDialog : Window
    {
        public AddClientViewModel ViewModel => (AddClientViewModel)DataContext;

        public AddClientDialog()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ViewModel.ClientName))
            {
                MessageBox.Show("Please enter a client name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
