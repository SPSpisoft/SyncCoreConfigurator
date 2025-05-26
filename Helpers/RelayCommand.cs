using System;
using System.Windows.Input;

namespace SyncCore.Helpers
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the CanExecute
    /// method is 'true'.
    /// </summary>
    /// <remarks>
    /// یک دستور که هدف آن انتقال عملکرد خود به سایر اشیاء با فراخوانی نماینده‌ها است
    /// </remarks>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <remarks>
        /// ایجاد یک دستور جدید که می‌تواند در هر زمانی اجرا شود
        /// </remarks>
        public RelayCommand(Action execute)
            : this(p => execute(), null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <remarks>
        /// ایجاد یک دستور جدید
        /// </remarks>
        public RelayCommand(Action execute, Func<bool> canExecute)
            : this(p => execute(), canExecute == null ? null : p => canExecute())
        {
        }

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <remarks>
        /// ایجاد یک دستور جدید که می‌تواند در هر زمانی اجرا شود
        /// </remarks>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <remarks>
        /// ایجاد یک دستور جدید
        /// </remarks>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        /// <remarks>
        /// زمانی رخ می‌دهد که تغییراتی که بر اجرای دستور تأثیر می‌گذارند رخ می‌دهند
        /// </remarks>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        /// <remarks>
        /// متدی که تعیین می‌کند آیا دستور می‌تواند در وضعیت فعلی خود اجرا شود
        /// </remarks>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <remarks>
        /// متدی که هنگام فراخوانی دستور فراخوانی می‌شود
        /// </remarks>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
} 