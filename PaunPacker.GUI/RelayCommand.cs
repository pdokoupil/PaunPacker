using System;

using System.Windows.Input;

namespace PaunPacker
{
    /// <summary>
    /// Implementation of ICommand
    /// </summary>
    /// <remarks>Based on https://msdn.microsoft.com/en-us/magazine/dd419663.aspx</remarks>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Constructs the relay command
        /// </summary>
        /// <param name="execute">The body of the command</param>
        /// <param name="canExecute">Predicate determining whether the command could be executed</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Event that is raised when the state of "can execute" has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Checkes whether a command could be executed (for a given parameter)
        /// </summary>
        /// <returns>True if the command can be executed, false otherwise</returns>
        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">The parameter that should be passed to the command</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;
    }
}
