using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;

namespace ThermostatCore.Common
{
    public class ViewModelBinder
    {
        private ViewModelBinder()
        {
        }

        private static ViewModelBinder __default;

        public static ViewModelBinder Default
        {
            get { return ViewModelBinder.__default; }
            set { ViewModelBinder.__default = value; }
        }

        static ViewModelBinder()
        {
            __default = new ViewModelBinder();
        }

        public static ViewModelBinder SetCurrent(ViewModelBinder viewModelBinder)
        {
            if (viewModelBinder == null)
                throw new ArgumentException("viewModelBinder was not specified");
            var lastBinder = __default;
            __default = viewModelBinder;
            return lastBinder;
        }
    }
}
