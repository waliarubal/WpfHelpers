using NullVoidCreations.WpfHelpers.Base;

namespace NullVoidCreations.WpfHelpers.DataStructures
{
    public class Triplet<F, S, T> : NotificationBase
    {
        F _first;
        S _second;
        T _third;

        public Triplet()
        { }

        public Triplet(F first, S second, T third, bool raiseNotification) : this()
        {
            RaiseNotifiation = raiseNotification;
            First = first;
            Second = second;
            Third = third;
        }

        public Triplet(F first, S second, T third) : this(first, second, third, true)
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

        public T Third
        {
            get { return _third; }
            set { Set(nameof(Third), ref _third, value, RaiseNotifiation); }
        }

        #endregion
    }
}
