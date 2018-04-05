using System.Reflection;
using System.Windows.Controls;

namespace NullVoidCreations.WpfHelpers.Base
{
    public abstract class ViewModelBase: NotificationBase
    {
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
    }
}
