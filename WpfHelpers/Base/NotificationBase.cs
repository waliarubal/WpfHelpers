using System.ComponentModel;

namespace NullVoidCreations.WpfHelpers.Base
{
    public abstract class NotificationBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(string propertyName, ref T backingField, T value, bool raiseNotification = true)
        {
            if (string.IsNullOrEmpty(propertyName))
                return false;

            if (value == null && backingField == null)
                return false;
            else if (value != null && value.Equals(backingField))
                return false;

            backingField = value;
            if (raiseNotification)
                RaisePropertyChanged(propertyName);
            return true;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler == null)
                return;

            if (string.IsNullOrEmpty(propertyName))
                return;

            handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
