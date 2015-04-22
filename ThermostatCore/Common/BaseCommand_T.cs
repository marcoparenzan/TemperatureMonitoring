using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ThermostatCore.Common
{
    public abstract class BaseCommand<T> : ICommand
        where T : BaseArgs
    {
        public BaseCommand()
        {
            _canExecuteCached = OnCanExecute(default(T));
        }

        protected bool? _canExecuteCached;

        public bool? CanExecuteCached
        {
            get { return _canExecuteCached; }
            set
            {
                if (_canExecuteCached != value)
                {
                    _canExecuteCached = value;
                    NotifyCanExecuteChanged();
                }
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return OnCanExecute((T)parameter);
        }

        protected abstract bool OnCanExecute(T parameter);

        private EventHandler _canExecuteChanged;

        event EventHandler ICommand.CanExecuteChanged
        {
            add { _canExecuteChanged += value; }
            remove { _canExecuteChanged -= value; }
        }

        protected virtual void NotifyCanExecuteChanged()
        {
            if (_canExecuteChanged != null)
            {
                try
                {
                    _canExecuteChanged(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        void ICommand.Execute(object parameter)
        {
            T args = (T)parameter;

            OnExecute(args);
        }

        protected abstract void OnExecute(T parameter);

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
