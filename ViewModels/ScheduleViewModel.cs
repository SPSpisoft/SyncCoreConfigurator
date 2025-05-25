using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SyncCore.Helpers;

namespace SyncCore.ViewModels
{
    public class ScheduleViewModel : ViewModelBase
    {
        private string _syncFrequency;
        private DateTime _startTime;
        private string _selectedDays;
        private int _interval;

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

        public ObservableCollection<string> FrequencyTypes { get; }

        public ObservableCollection<string> Days { get; }

        public ScheduleViewModel()
        {
            FrequencyTypes = new ObservableCollection<string>
            {
                "Daily",
                "Weekly",
                "Monthly"
            };
            SyncFrequency = "Daily";

            Days = new ObservableCollection<string>
            {
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday"
            };

            StartTime = DateTime.Now;
            Interval = 1;
        }
    }
} 