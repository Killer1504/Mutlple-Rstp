using System.Windows;
using MultiRtspViewer.ViewModels;

namespace MultiRtspViewer.Views
{
    public partial class BatchImportDialog : Window
    {
        public BatchImportViewModel ViewModel => (BatchImportViewModel)DataContext;

        public BatchImportDialog()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
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
