using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SyncCore.Helpers;

namespace SyncCore.ViewModels
{
    public class ReviewViewModel : ViewModelBase
    {
        private string _sourceServer;
        private string _sourceDatabase;
        private string _targetServer;
        private string _targetDatabase;
        private ObservableCollection<string> _selectedTables;
        private string _syncFrequency;
        private DateTime _startTime;
        private string _selectedDays;
        private int _interval;

        public string SourceServer
        {
            get => _sourceServer;
            set => SetProperty(ref _sourceServer, value);
        }

        public string SourceDatabase
        {
            get => _sourceDatabase;
            set => SetProperty(ref _sourceDatabase, value);
        }

        public string TargetServer
        {
            get => _targetServer;
            set => SetProperty(ref _targetServer, value);
        }

        public string TargetDatabase
        {
            get => _targetDatabase;
            set => SetProperty(ref _targetDatabase, value);
        }

        public ObservableCollection<string> SelectedTables
        {
            get => _selectedTables;
            set => SetProperty(ref _selectedTables, value);
        }

        public string SyncFrequency
        {
            get => _syncFrequency;
            set => SetProperty(ref _syncFrequency, value);
        }

        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        public string SelectedDays
        {
            get => _selectedDays;
            set => SetProperty(ref _selectedDays, value);
        }

        public int Interval
        {
            get => _interval;
            set => SetProperty(ref _interval, value);
        }

        public ReviewViewModel()
        {
            SelectedTables = new ObservableCollection<string>();
        }
    }
} 