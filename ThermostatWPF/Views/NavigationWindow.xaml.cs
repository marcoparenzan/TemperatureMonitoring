using System;
using System.Windows;
using System.Windows.Controls;
using ThermostatCore.Common;

namespace ThermostatWPF.Views
{
    /// <summary>
    /// Interaction logic for NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window, INavigation
    {
        public NavigationWindow()
        {
            InitializeComponent();    
        }

        INavigation INavigation.ViewModel<TViewModel>(params object[] viewModelargs)
        {
            var viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), this, viewModelargs);
            this.DataContext = viewModel;
            return this;
        }

        INavigation INavigation.ViewModel<TViewModel>(TViewModel viewModel)
        {
            this.DataContext = viewModel;
            return this;
        }

        INavigation INavigation.ViewModel<TViewModel>(Action<TViewModel> init, params object[] viewModelargs)
        {
            var viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), this, viewModelargs);
            if (init != null)
            {
                init(viewModel);
            }
            this.DataContext = viewModel;
            return this;
        }

        private UIElement _current;

        INavigation INavigation.View(string viewName)
        {
            ((INavigation)this).Dismiss();

            var type = Type.GetType(this.GetType().Namespace + "." + viewName);
            var view = (UIElement)Activator.CreateInstance(type);
            if (!(view is UIElement)) throw new InvalidCastException("View is not UIElement");
            _current = (UIElement)(object)view;

            Grid.SetColumn(_current, 0);
            Grid.SetRow(_current, 1);
            g.Children.Add(_current);
            g.UpdateLayout();

            return this;
        }

        INavigation INavigation.Dismiss()
        {
            if (_current != null)
            {
                g.Children.Remove(_current);
                g.UpdateLayout();
            }
            if (this.DataContext != null)
            {
                this.DataContext = null;
            }
            return this;
        }

        void INavigation.Close()
        {
            throw new NotImplementedException();
        }

        void INavigation.Invoke(Action a)
        {
            Dispatcher.Invoke(a);
        }
    }
}
