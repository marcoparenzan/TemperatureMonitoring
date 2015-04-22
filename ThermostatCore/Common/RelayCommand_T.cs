using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ThermostatCore.Common
{
    public class RelayCommand<T> : BaseCommand<T>
        where T : BaseArgs
    {
        private Action<T> _execute;
        private Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute = null, Predicate<T> canExecute = null)
        {
            if (execute != null)
                _execute = execute;
            else
                _execute = arg => { };
            if (canExecute != null)
                _canExecute = canExecute;
            else
                _canExecute = arg => true;
        }

        protected override bool OnCanExecute(T parameter)
        {
            if (_canExecute == null) return true;
            var thisCanExecute = _canExecute(parameter);
            if (this.CanExecuteCached != thisCanExecute)
            {
                this._canExecuteCached = thisCanExecute;
                this.NotifyCanExecuteChanged();
            }

            return thisCanExecute;
        }

        protected override void OnExecute(T parameter)
        {
            _execute(parameter);
        }
    }
}
