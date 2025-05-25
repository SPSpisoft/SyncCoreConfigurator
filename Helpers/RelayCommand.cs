using System;
using System.Windows.Input;

namespace SyncCore.Helpers
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates
    /// </summary>
    /// <remarks>
    /// یک دستور که هدف آن انتقال عملکرد خود به سایر اشیاء با فراخوانی نماینده‌ها است
    /// </remarks>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        /// <summary>
        /// Creates a new command
        /// </summary>
        /// <param name="execute">The execution logic</param>
        /// <param name="canExecute">The execution status logic</param>
        /// <remarks>
        /// ایجاد یک دستور جدید
        /// </remarks>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute
        /// </summary>
        /// <remarks>
        /// زمانی رخ می‌دهد که تغییراتی که بر اجرای دستور تأثیر می‌گذارند رخ می‌دهند
        /// </remarks>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <returns>True if this command can be executed</returns>
        /// <remarks>
        /// متدی که تعیین می‌کند آیا دستور می‌تواند در وضعیت فعلی خود اجرا شود
        /// </remarks>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <remarks>
        /// متدی که هنگام فراخوانی دستور فراخوانی می‌شود
        /// </remarks>
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Creates a new command
        /// </summary>
        /// <param name="execute">The execution logic</param>
        /// <param name="canExecute">The execution status logic</param>
        /// <remarks>
        /// ایجاد یک دستور جدید
        /// </remarks>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute
        /// </summary>
        /// <remarks>
        /// زمانی رخ می‌دهد که تغییراتی که بر اجرای دستور تأثیر می‌گذارند رخ می‌دهند
        /// </remarks>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <returns>True if this command can be executed</returns>
        /// <remarks>
        /// متدی که تعیین می‌کند آیا دستور می‌تواند در وضعیت فعلی خود اجرا شود
        /// </remarks>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <remarks>
        /// متدی که هنگام فراخوانی دستور فراخوانی می‌شود
        /// </remarks>
        public void Execute(object parameter)
        {
            _execute();
        }
    }
} 