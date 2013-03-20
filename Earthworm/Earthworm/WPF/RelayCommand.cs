using System;
using System.Windows.Input;

namespace Earthworm.WPF
{
    /// <summary>
    /// An ICommand object generated from delegates.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class from delegate type objects.
        /// </summary>
        /// <param name="execute">An action that is to be executed.</param>
        /// <param name="canExecute">A predicate that indicates if the action can be executed.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            if (execute == null)
                throw new Exception("The action for the relay command cannot be null.");

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class from delegate type objects.
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(o => execute(), canExecute == null ? null as Func<object, bool> : o => canExecute())
        {
        }

        /// <summary>
        /// Indicates if the action can be executed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool CanExecute(object obj)
        {
            return _canExecute == null || _canExecute(obj);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="obj"></param>
        public void Execute(object obj)
        {
            _execute(obj);
        }
    }
}
