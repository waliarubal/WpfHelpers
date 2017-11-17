using System.ComponentModel;

namespace NullVoidCreations.WpfHelpers.Base
{
    public abstract class NotificationBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(string propertyName, ref T backingField, T value, bool raiseNotification = true)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            if (value == null && backingField == null)
                return;
            else if (value != null && value.Equals(backingField))
                return;

            backingField = value;
            if (raiseNotification)
                RaisePropertyChanged(propertyName);
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
