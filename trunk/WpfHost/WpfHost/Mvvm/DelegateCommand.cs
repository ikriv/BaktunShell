// Copyright (c) 2013 Ivan Krivyakov
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
using System;
using System.Windows.Input;

namespace IKriv.WpfHost.Mvvm
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _action;
        private readonly Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> action)
            :
            this(action, null)
        {
        }

        public DelegateCommand(Action<T> action, Func<T, bool> canExecute)
        {
            if (action == null) throw new ArgumentNullException("action");
            _action = action;
            _canExecute = canExecute ?? (anything => true);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action action)
            :
            this(action, null)
        {
        }

        public DelegateCommand(Action action, Func<bool> canExecute)
            :
            base(EnsureNotNull(action), SafeConvert(canExecute))
        {
        }

        private static Action<object> EnsureNotNull(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            return unusedArg => action();
        }

        private static Func<object, bool> SafeConvert(Func<bool> canExecute)
        {
            if (canExecute == null) return null;
            return unusedArg => canExecute();
        }
    }
}
