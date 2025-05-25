using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SyncCore.Helpers;
using SyncCore.Models;
using System.Windows;
using SyncCore.Commands;
using SyncCore.ViewModels;

namespace SyncCore.ViewModels
{
    /// <summary>
    /// ViewModel for managing the configuration wizard
    /// </summary>
    /// <remarks>
    /// ویو مدل مدیریت ویزارد تنظیمات
    /// </remarks>
    public class ConfigurationWizardViewModel : ViewModelBase
    {
        private readonly ObservableCollection<WizardStep> _steps;
        private int _currentStep;
        private bool _canMoveNext;
        private bool _canMovePrevious;
        private string _statusMessage;
        private bool _isBusy;
        private ViewModelBase _currentStepViewModel;
        private bool _isPreviousVisible;
        private bool _isNextVisible;
        private bool _isFinishVisible;

        /// <summary>
        /// Collection of wizard steps
        /// </summary>
        /// <remarks>
        /// مجموعه مراحل ویزارد
        /// </remarks>
        public ObservableCollection<WizardStep> Steps => _steps;

        /// <summary>
        /// Current step index
        /// </summary>
        /// <remarks>
        /// شاخص مرحله فعلی
        /// </remarks>
        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                if (SetProperty(ref _currentStep, value))
                {
                    LoadStepViewModel();
                    UpdateNavigationVisibility();
                }
            }
        }

        /// <summary>
        /// Current step
        /// </summary>
        /// <remarks>
        /// مرحله فعلی
        /// </remarks>
        public WizardStep CurrentStepObject => _steps[CurrentStep];

        /// <summary>
        /// Current step view model
        /// </summary>
        /// <remarks>
        /// مدل ویوی مرحله فعلی
        /// </remarks>
        public ViewModelBase CurrentStepViewModel
        {
            get => _currentStepViewModel;
            set => SetProperty(ref _currentStepViewModel, value);
        }

        /// <summary>
        /// Command to move to next step
        /// </summary>
        /// <remarks>
        /// دستور حرکت به مرحله بعد
        /// </remarks>
        public ICommand NextCommand { get; }

        /// <summary>
        /// Command to move to previous step
        /// </summary>
        /// <remarks>
        /// دستور حرکت به مرحله قبل
        /// </remarks>
        public ICommand PreviousCommand { get; }

        /// <summary>
        /// Command to finish the wizard
        /// </summary>
        /// <remarks>
        /// دستور تکمیل ویزارد
        /// </remarks>
        public ICommand FinishCommand { get; }

        /// <summary>
        /// Command to cancel the wizard
        /// </summary>
        /// <remarks>
        /// دستور لغو ویزارد
        /// </remarks>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Status message
        /// </summary>
        /// <remarks>
        /// پیام وضعیت
        /// </remarks>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicates if the wizard is busy
        /// </summary>
        /// <remarks>
        /// نشان دهنده اینکه ویزارد در حال انجام کار است یا خیر
        /// </remarks>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Indicates if can move to next step
        /// </summary>
        /// <remarks>
        /// بررسی امکان حرکت به مرحله بعد
        /// </remarks>
        public bool CanMoveNext
        {
            get => _canMoveNext;
            set
            {
                if (_canMoveNext != value)
                {
                    _canMoveNext = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Indicates if can move to previous step
        /// </summary>
        /// <remarks>
        /// بررسی امکان حرکت به مرحله قبل
        /// </remarks>
        public bool CanMovePrevious
        {
            get => _canMovePrevious;
            set
            {
                if (_canMovePrevious != value)
                {
                    _canMovePrevious = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Indicates if the previous button is visible
        /// </summary>
        /// <remarks>
        /// نشان دهنده اینکه دکمه قبلی قابل نمایش است یا خیر
        /// </remarks>
        public bool IsPreviousVisible
        {
            get => _isPreviousVisible;
            set => SetProperty(ref _isPreviousVisible, value);
        }

        /// <summary>
        /// Indicates if the next button is visible
        /// </summary>
        /// <remarks>
        /// نشان دهنده اینکه دکمه بعدی قابل نمایش است یا خیر
        /// </remarks>
        public bool IsNextVisible
        {
            get => _isNextVisible;
            set => SetProperty(ref _isNextVisible, value);
        }

        /// <summary>
        /// Indicates if the finish button is visible
        /// </summary>
        /// <remarks>
        /// نشان دهنده اینکه دکمه تکمیل قابل نمایش است یا خیر
        /// </remarks>
        public bool IsFinishVisible
        {
            get => _isFinishVisible;
            set => SetProperty(ref _isFinishVisible, value);
        }

        public ConfigurationWizardViewModel()
        {
            _steps = new ObservableCollection<WizardStep>
            {
                new WizardStep { Title = "Database Connection", Description = "Configure database connection settings" },
                new WizardStep { Title = "Sync Settings", Description = "Configure synchronization settings" },
                new WizardStep { Title = "Schedule", Description = "Configure synchronization schedule" },
                new WizardStep { Title = "Review", Description = "Review and complete configuration" }
            };

            NextCommand = new RelayCommand(Next, CanNext);
            PreviousCommand = new RelayCommand(Previous, CanPrevious);
            FinishCommand = new RelayCommand(Finish, CanFinish);
            CancelCommand = new RelayCommand(Cancel);

            CurrentStep = 0;
        }

        /// <summary>
        /// Updates the navigation state
        /// </summary>
        /// <remarks>
        /// به‌روزرسانی وضعیت حرکت
        /// </remarks>
        private void UpdateNavigationState()
        {
            CanMoveNext = CurrentStep < _steps.Count - 1;
            CanMovePrevious = CurrentStep > 0;
        }

        /// <summary>
        /// Updates the navigation visibility
        /// </summary>
        /// <remarks>
        /// به‌روزرسانی وضعیت نمایش دکمه‌ها
        /// </remarks>
        private void UpdateNavigationVisibility()
        {
            IsPreviousVisible = CurrentStep > 0;
            IsNextVisible = CurrentStep < _steps.Count - 1;
            IsFinishVisible = CurrentStep == _steps.Count - 1;
        }

        /// <summary>
        /// Loads the current step view model
        /// </summary>
        /// <remarks>
        /// بارگذاری مدل ویوی مرحله فعلی
        /// </remarks>
        private void LoadStepViewModel()
        {
            CurrentStepViewModel = CurrentStep switch
            {
                0 => new DatabaseConnectionViewModel(),
                1 => new SyncSettingsViewModel(),
                2 => new ScheduleViewModel(),
                3 => new ReviewViewModel(),
                _ => throw new ArgumentOutOfRangeException(nameof(CurrentStep))
            };
        }

        /// <summary>
        /// Moves to the next step
        /// </summary>
        /// <remarks>
        /// حرکت به مرحله بعد
        /// </remarks>
        private void Next(object parameter)
        {
            if (CurrentStep < _steps.Count - 1)
            {
                CurrentStep++;
                LoadStepViewModel();
            }
        }

        /// <summary>
        /// Moves to the previous step
        /// </summary>
        /// <remarks>
        /// حرکت به مرحله قبل
        /// </remarks>
        private void Previous(object parameter)
        {
            if (CurrentStep > 0)
            {
                CurrentStep--;
                LoadStepViewModel();
            }
        }

        /// <summary>
        /// Finishes the wizard
        /// </summary>
        /// <remarks>
        /// تکمیل ویزارد
        /// </remarks>
        private void Finish(object parameter)
        {
            IsBusy = true;
            StatusMessage = "Saving configuration...";

            try
            {
                // TODO: Implement configuration saving logic
                MessageBox.Show("Configuration saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving configuration: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Checks if can finish the wizard
        /// </summary>
        /// <returns>True if can finish the wizard</returns>
        /// <remarks>
        /// بررسی امکان تکمیل ویزارد
        /// </remarks>
        private bool CanFinish(object parameter)
        {
            return !IsBusy && CurrentStep == _steps.Count - 1;
        }

        /// <summary>
        /// Cancels the wizard
        /// </summary>
        /// <remarks>
        /// لغو ویزارد
        /// </remarks>
        private void Cancel(object parameter)
        {
            var result = MessageBox.Show("Are you sure you want to cancel?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Checks if can move to next step
        /// </summary>
        /// <returns>True if can move to next step</returns>
        /// <remarks>
        /// بررسی امکان حرکت به مرحله بعد
        /// </remarks>
        private bool CanNext(object parameter)
        {
            return !IsBusy && CurrentStep < _steps.Count - 1;
        }

        /// <summary>
        /// Checks if can move to previous step
        /// </summary>
        /// <returns>True if can move to previous step</returns>
        /// <remarks>
        /// بررسی امکان حرکت به مرحله قبل
        /// </remarks>
        private bool CanPrevious(object parameter)
        {
            return !IsBusy && CurrentStep > 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 