using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NullVoidCreations.WpfHelpers.Behaviors
{
    // taken from: https://www.codeproject.com/Articles/389764/A-Smart-Behavior-for-DataGrid-AutoGenerateColumn
    public class ColumnHeaderBehavior: Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += OnAutoGeneratingColumn;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -= OnAutoGeneratingColumn;
        }

        protected void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var displayName = GetPropertyDisplayName(e.PropertyDescriptor);
            if (!string.IsNullOrEmpty(displayName))
                e.Column.Header = displayName;
            else
                e.Cancel = true;
        }

        protected static string GetPropertyDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;
            if (pd != null)
            {
                var attr = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
                if (attr != null && attr != DisplayNameAttribute.Default)
                    return attr.DisplayName;
            }

            else
            {
                var pi = descriptor as PropertyInfo;
                if (pi != null)
                {
                    var attrs = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    foreach (var att in attrs)
                    {
                        var attribute = att as DisplayNameAttribute;
                        if (attribute != null && attribute != DisplayNameAttribute.Default)
                            return attribute.DisplayName;
                    }
                }
            }

            return null;
        }
    }
}
