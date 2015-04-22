using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ThermostatCore.Common
{
    public class RelayCommand : BaseCommand
    {
        private Action<object> _execute;
        private Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
            _canExecute = _ => true;
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool OnCanExecute(object parameter)
        {
            var thisCanExecute = _canExecute(parameter);
            if (this.CanExecuteCached != thisCanExecute)
                NotifyCanExecuteChanged();

            return thisCanExecute;
        }

        protected override void OnExecute(object parameter)
        {
            _execute(parameter);
        }
    }
}
