using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace NullVoidCreations.WpfHelpers.Base
{
    public abstract class ViewModelBase: NotificationBase
    {
        bool _isDialog;
        object _result;
        Window _container;

        public Control GetView()
        {
            var viewModelName = GetType().Name;
            var viewName = viewModelName.Replace("ViewModel", "View");

            var assembly = Assembly.GetCallingAssembly();
            foreach(var type in assembly.GetTypes())
            {
                if (type.IsClass && type.Name.Equals(viewName))
                {
                    var view = assembly.CreateInstance(type.FullName) as Control;
                    view.DataContext = this;
                    return view;
                }
            }

            return null;
        }

        public TResult ShowDialog<TResult>(Control view, Window owner = null)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            _isDialog = !_isDialog;

            _container = new Window();
            if (owner != null)
            {
                _container.ShowInTaskbar = false;
                _container.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _container.Owner = owner;
            }
            else
                _container.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _container.SizeToContent = SizeToContent.WidthAndHeight;
            _container.WindowStyle = WindowStyle.SingleBorderWindow;
            _container.ResizeMode = ResizeMode.NoResize;
            _container.Content = view;
            _container.ShowDialog();

            return (TResult)_result;
        }

        protected void Close<TResult>(TResult result)
        {
            if (!_isDialog)
                throw new InvalidOperationException("Operation is only allowed when open using ShowDialog.");

            _result = result;
            _container.Close();
            _container = null;

            _isDialog = !_isDialog;
        }
    }
}
