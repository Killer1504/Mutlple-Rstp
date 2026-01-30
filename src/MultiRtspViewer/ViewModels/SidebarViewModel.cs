using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiRtspViewer.Models;
using MultiRtspViewer.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MultiRtspViewer.ViewModels
{
    public partial class SidebarViewModel : ObservableObject
    {
        private readonly ClientService _clientService;
        
        [ObservableProperty]
        private ObservableCollection<ClientModel> clients = new();

        [ObservableProperty]
        private ClientModel? selectedClient;

        public SidebarViewModel()
        {
            _clientService = new ClientService();
            LoadClients();
        }

        public void LoadClients()
        {
            var dbClients = _clientService.GetAllClients();
            Clients.Clear();
            foreach (var dbClient in dbClients)
            {
                Clients.Add(new ClientModel
                {
                    Id = dbClient.Id,
                    Name = dbClient.Name,
                    Description = dbClient.Description,
                    CameraCount = dbClient.Cameras.Count
                });
            }

            // Auto-select first client if available
            if (Clients.Count > 0)
            {
                SelectedClient = Clients[0];
            }
        }

        partial void OnSelectedClientChanged(ClientModel? value)
        {
            // Set IsSelected flag for UI styling
            foreach (var client in Clients)
            {
                client.IsSelected = (client == value);
            }
            
            // Notify MainViewModel (This will be wired up later via event or shared service)
        }

        [RelayCommand]
        public void AddClient()
        {
            var dialog = new Views.AddClientDialog
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                var vm = dialog.ViewModel;
                var dbClient = _clientService.CreateClient(vm.ClientName.Trim(), ""); // CreateClient takes name, desc
                LoadClients();
                
                // Select the new client
                SelectedClient = Clients.FirstOrDefault(c => c.Id == dbClient.Id);
            }
        }

        [RelayCommand]
        public void ManageClients()
        {
            var dialog = new Views.ClientManagerDialog
            {
                Owner = Application.Current.MainWindow
            };
            
            dialog.ShowDialog();
            
            // Refresh list to reflect changes (Renames/Deletes)
            LoadClients();
        }

        [RelayCommand]
        public void DeleteClient(ClientModel client)
        {
            if (client == null) return;
            // Legacy/Fallback command
             var result = MessageBox.Show(
                $"Are you sure you want to delete '{client.Name}'?\nAll associated cameras will be lost.",
                "Delete Client",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _clientService.DeleteClient(client.Id);
                LoadClients();
            }
        }
        [RelayCommand]
        public void OpenAbout()
        {
            var dialog = new Views.AboutDialog
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }
    }
}
