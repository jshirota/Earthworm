using System;
using System.Windows.Input;

namespace EarthwormUI
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            if (execute == null)
                throw new Exception("The action for the relay command cannot be null.");

            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(o => execute(), canExecute == null ? null as Func<object, bool> : o => canExecute())
        {
        }

        public bool CanExecute(object obj)
        {
            return _canExecute == null || _canExecute(obj);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object obj)
        {
            _execute(obj);
        }
    }
}
