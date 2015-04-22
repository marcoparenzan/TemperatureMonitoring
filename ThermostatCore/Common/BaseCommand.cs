using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ThermostatCore.Common
{
    public abstract class BaseCommand : ICommand
    {
        public BaseCommand()
        {
            _canExecuteCached = true;
        }

        private bool? _canExecuteCached;

        public bool? CanExecuteCached
        {
            get { return _canExecuteCached; }
            set
            {
                _canExecuteCached = value;
                NotifyCanExecuteChanged();
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return OnCanExecute(parameter);
        }

        protected abstract bool OnCanExecute(object parameter);

        private EventHandler _canExecuteChanged;

        event EventHandler ICommand.CanExecuteChanged
        {
            add { _canExecuteChanged += value; }
            remove { _canExecuteChanged -= value; }
        }

        protected void NotifyCanExecuteChanged()
        {
            if (_canExecuteChanged != null)
                _canExecuteChanged(this, EventArgs.Empty);
        }

        void ICommand.Execute(object parameter)
        {
            OnExecute(parameter);
        }

        protected abstract void OnExecute(object parameter);

        public void Execute()
        {
            OnExecute(null);
        }

        public bool CanExecute()
        {
            return OnCanExecute(null);
        }
    }
}
