using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SyncCore.Helpers;
using SyncCore.Commands;
using SyncCore.Models;

namespace SyncCore.ViewModels
{
    public class SyncSettingsViewModel : ViewModelBase
    {
        private ObservableCollection<DatabaseConnection> _availableDatabases;
        private DatabaseConnection _sourceDatabase;
        private DatabaseConnection _targetDatabase;
        private ObservableCollection<string> _availableTables;
        private ObservableCollection<string> _selectedTables;
        private bool _isLoading;
        private string _selectedAvailableTable;
        private string _selectedTable;

        public ObservableCollection<DatabaseConnection> AvailableDatabases
        {
            get => _availableDatabases;
            set => SetProperty(ref _availableDatabases, value);
        }

        public DatabaseConnection SourceDatabase
        {
            get => _sourceDatabase;
            set
            {
                if (SetProperty(ref _sourceDatabase, value))
                {
                    LoadAvailableTables();
                }
            }
        }

        public DatabaseConnection TargetDatabase
        {
            get => _targetDatabase;
            set => SetProperty(ref _targetDatabase, value);
        }

        public ObservableCollection<string> AvailableTables
        {
            get => _availableTables;
            set => SetProperty(ref _availableTables, value);
        }

        public ObservableCollection<string> SelectedTables
        {
            get => _selectedTables;
            set => SetProperty(ref _selectedTables, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string SelectedAvailableTable
        {
            get => _selectedAvailableTable;
            set => SetProperty(ref _selectedAvailableTable, value);
        }

        public string SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
        }

        public ICommand AddTableCommand { get; }
        public ICommand RemoveTableCommand { get; }

        public SyncSettingsViewModel()
        {
            AvailableDatabases = new ObservableCollection<DatabaseConnection>();
            AvailableTables = new ObservableCollection<string>();
            SelectedTables = new ObservableCollection<string>();
            AddTableCommand = new RelayCommand(AddTable, CanAddTable);
            RemoveTableCommand = new RelayCommand(RemoveTable, CanRemoveTable);
        }

        private bool CanAddTable()
        {
            return !string.IsNullOrEmpty(SelectedAvailableTable) && 
                   !SelectedTables.Contains(SelectedAvailableTable);
        }

        private void AddTable()
        {
            if (CanAddTable())
            {
                SelectedTables.Add(SelectedAvailableTable);
                AvailableTables.Remove(SelectedAvailableTable);
                SelectedAvailableTable = null;
            }
        }

        private bool CanRemoveTable()
        {
            return !string.IsNullOrEmpty(SelectedTable);
        }

        private void RemoveTable()
        {
            if (CanRemoveTable())
            {
                AvailableTables.Add(SelectedTable);
                SelectedTables.Remove(SelectedTable);
                SelectedTable = null;
            }
        }

        private async void LoadAvailableTables()
        {
            if (SourceDatabase == null) return;

            IsLoading = true;
            AvailableTables.Clear();

            try
            {
                using (var connection = new SqlConnection(SourceDatabase.ConnectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'",
                        connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AvailableTables.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle error
                System.Diagnostics.Debug.WriteLine($"Error loading tables: {ex.Message}");
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
                DataSource = ".",
                InitialCatalog = SourceDatabase.DatabaseName,
                IntegratedSecurity = true
            };

            return builder.ConnectionString;
        }
    }
} 