using System.Windows;
using MultiRtspViewer.ViewModels;

namespace MultiRtspViewer.Views
{
    public partial class ClientManagerDialog : Window
    {
        public ClientManagerViewModel ViewModel => (ClientManagerViewModel)DataContext;

        public ClientManagerDialog()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
