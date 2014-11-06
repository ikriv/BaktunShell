using System;
using System.Windows.Input;

namespace Shell.Util
{
    class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _action;

        public DelegateCommand(Action<T> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }
    }

    class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action action)
            :
            base( unused=>action())
        {}
    }
}
