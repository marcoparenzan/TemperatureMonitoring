﻿/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2015 Marco Parenzan (https://it.linkedin.com/in/marcoparenzan)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
