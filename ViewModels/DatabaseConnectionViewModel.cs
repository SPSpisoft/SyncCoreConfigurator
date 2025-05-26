using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Input;
using SyncCore.Helpers;
using SyncCore.Models;

namespace SyncCore.ViewModels
{
    public class DatabaseConnectionViewModel : ViewModelBase
    {
        private string _serverName;
        private string _authentication;
        private string _username;
        private string _password;
        private bool _isLoading;
        private ObservableCollection<SqlServerInstance> _availableServers;
        private ObservableCollection<string> _availableDatabases;

        public string ServerName
        {
            get => _serverName;
            set
            {
                if (SetProperty(ref _serverName, value))
                {
                    LoadAvailableDatabases();
                }
            }
        }

        public string Authentication
        {
            get => _authentication;
            set => SetProperty(ref _authentication, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<SqlServerInstance> AvailableServers
        {
            get => _availableServers;
            set => SetProperty(ref _availableServers, value);
        }

        public ObservableCollection<string> AvailableDatabases
        {
            get => _availableDatabases;
            set => SetProperty(ref _availableDatabases, value);
        }

        public ICommand LoadServersCommand { get; }

        public DatabaseConnectionViewModel()
        {
            _availableServers = new ObservableCollection<SqlServerInstance>();
            _availableDatabases = new ObservableCollection<string>();
            _authentication = "Windows Authentication";
            LoadServersCommand = new RelayCommand(async _ => await LoadServers());
            LoadServersCommand.Execute(null);
        }

        private async Task LoadServers()
        {
            IsLoading = true;
            try
            {
                var servers = await SqlServerInstance.GetAvailableServers();
                AvailableServers.Clear();
                foreach (var server in servers)
                {
                    AvailableServers.Add(server);
                }
            }
            catch (Exception ex)
            {
                // Handle error
                System.Windows.MessageBox.Show($"Error loading servers: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadAvailableDatabases()
        {
            if (string.IsNullOrEmpty(ServerName))
                return;

            IsLoading = true;
            try
            {
                using (var connection = new SqlConnection(BuildConnectionString()))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(
                        "SELECT name FROM sys.databases WHERE database_id > 4",
                        connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var databases = new ObservableCollection<string>();
                            while (await reader.ReadAsync())
                            {
                                databases.Add(reader.GetString(0));
                            }
                            AvailableDatabases = databases;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle error
                System.Windows.MessageBox.Show($"Error loading databases: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private string BuildConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                IntegratedSecurity = Authentication == "Windows Authentication"
            };

            if (!builder.IntegratedSecurity)
            {
                builder.UserID = Username;
                builder.Password = Password;
            }

            return builder.ConnectionString;
        }
    }
}