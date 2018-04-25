using NullVoidCreations.WpfHelpers.Base;

namespace NullVoidCreations.WpfHelpers.DataStructures
{
    public class Doublet<F, S>: NotificationBase
    {
        F _first;
        S _second;

        public Doublet() { }

        public Doublet(F first, S second, bool raiseNotification): this()
        {
            RaiseNotifiation = raiseNotification;
        }

        public Doublet(F first, S second): this(first, second, true)
        {

        }

        #region properties

        public bool RaiseNotifiation { get; set; }

        public F First
        {
            get { return _first; }
            set { Set(nameof(First), ref _first, value, RaiseNotifiation); }
        }

        public S Second
        {
            get { return _second; }
            set { Set(nameof(Second), ref _second, value, RaiseNotifiation); }
        }

        #endregion
    }
}
