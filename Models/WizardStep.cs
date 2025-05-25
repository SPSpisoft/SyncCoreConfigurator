using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SyncCore.Models
{
    /// <summary>
    /// Represents a step in the configuration wizard
    /// </summary>
    /// <remarks>
    /// کلاس نمایش‌دهنده یک مرحله در ویزارد تنظیمات
    /// </remarks>
    public class WizardStep : INotifyPropertyChanged
    {
        private string _title;
        private string _description;
        private bool _isActive;
        private bool _isCompleted;

        /// <summary>
        /// Unique identifier for the step
        /// </summary>
        /// <remarks>
        /// شناسه یکتای مرحله
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// Title of the step
        /// </summary>
        /// <remarks>
        /// عنوان مرحله
        /// </remarks>
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Description of the step
        /// </summary>
        /// <remarks>
        /// توضیحات مرحله
        /// </remarks>
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicates if the step is completed
        /// </summary>
        /// <remarks>
        /// نشان‌دهنده تکمیل مرحله
        /// </remarks>
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicates if the step is the current active step
        /// </summary>
        /// <remarks>
        /// نشان‌دهنده فعال بودن مرحله
        /// </remarks>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 