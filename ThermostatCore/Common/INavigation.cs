using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThermostatCore.Common
{
    public interface INavigation
    {
        INavigation Dismiss();
        INavigation ViewModel<TViewModel>(TViewModel viewModel);
        INavigation ViewModel<TViewModel>(Action<TViewModel> init, params object[] viewModelArgs);
        INavigation ViewModel<TViewModel>(params object[] viewModelArgs);
        INavigation View(string viewName);
        void Close();

        void Invoke(Action a);
    }
}
