using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SyncCore.Helpers;
using SyncCore.Models;

namespace SyncCore.ViewModels
{
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        private readonly SqlServerInstance _sqlServerInstance;
        private string _selectedServer;
        private string _selectedDatabase;
        private bool _useWindowsAuth = true;
        private string _username;
        private string _password;
        private bool _isConnecting;
        private string _connectionStatus;

        public ConnectionViewModel()
        {
            _sqlServerInstance = new SqlServerInstance();
            TestConnectionCommand = new RelayCommand(TestConnection, CanTestConnection);
            LoadDatabasesCommand = new RelayCommand(LoadDatabases, CanLoadDatabases);
        }

        public List<string> AvailableServers => _sqlServerInstance.AvailableServers;
        public List<string> AvailableDatabases => _sqlServerInstance.AvailableDatabases;

        public string SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (_selectedServer != value)
                {
                    _selectedServer = value;
                    _sqlServerInstance.ServerName = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string SelectedDatabase
        {
            get => _selectedDatabase;
            set
            {
                if (_selectedDatabase != value)
                {
                    _selectedDatabase = value;
                    _sqlServerInstance.DatabaseName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool UseWindowsAuth
        {
            get => _useWindowsAuth;
            set
            {
                if (_useWindowsAuth != value)
                {
                    _useWindowsAuth = value;
                    _sqlServerInstance.UseWindowsAuth = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ShowCredentials));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool ShowCredentials => !UseWindowsAuth;

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    _sqlServerInstance.Username = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    _sqlServerInstance.Password = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsConnecting
        {
            get => _isConnecting;
            set
            {
                if (_isConnecting != value)
                {
                    _isConnecting = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus != value)
                {
                    _connectionStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand TestConnectionCommand { get; }
        public ICommand LoadDatabasesCommand { get; }

        private async void TestConnection(object parameter)
        {
            IsConnecting = true;
            ConnectionStatus = "Testing connection...";

            try
            {
                bool success = await System.Threading.Tasks.Task.Run(() => _sqlServerInstance.TestConnection());
                ConnectionStatus = success ? "Connection successful!" : "Connection failed.";
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
            }
            finally
            {
                IsConnecting = false;
            }
        }

        private bool CanTestConnection(object parameter)
        {
            return !IsConnecting && !string.IsNullOrEmpty(SelectedServer) &&
                   (UseWindowsAuth || (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)));
        }

        private async void LoadDatabases(object parameter)
        {
            IsConnecting = true;
            ConnectionStatus = "Loading databases...";

            try
            {
                await System.Threading.Tasks.Task.Run(() => _sqlServerInstance.LoadAvailableDatabases());
                OnPropertyChanged(nameof(AvailableDatabases));
                ConnectionStatus = "Databases loaded successfully.";
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error loading databases: {ex.Message}";
            }
            finally
            {
                IsConnecting = false;
            }
        }

        private bool CanLoadDatabases(object parameter)
        {
            return !IsConnecting && !string.IsNullOrEmpty(SelectedServer) &&
                   (UseWindowsAuth || (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
