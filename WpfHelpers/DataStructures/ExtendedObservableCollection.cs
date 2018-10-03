using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace NullVoidCreations.WpfHelpers.DataStructures
{
    public class ExtendedObservableCollection<T>: ObservableCollection<T>
    {
        readonly Func<string> _toStringOverride;
        readonly PropertyChangedEventArgs _toStringEventArgs;

        #region constructors

        public ExtendedObservableCollection()
        {
            _toStringEventArgs = new PropertyChangedEventArgs(nameof(String));
            _toStringOverride = ToStringDefault;
        }

        public ExtendedObservableCollection(IEnumerable<T> values): this()
        {
            foreach (T item in values)
                Add(item);
        }

        public ExtendedObservableCollection(Func<string> toStringOverride)
        {
            _toStringOverride = toStringOverride;
        }

        #endregion

        #region properties

        public string String
        {
            get { return ToString(); }
        }

        #endregion

        string  ToStringDefault()
        {
            if (Count == 0)
                return default(string);

            var builder = new StringBuilder();
            foreach (var item in Items)
                builder.AppendFormat("{0},", item);
            if (builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public override string ToString()
        {
            return _toStringOverride.Invoke();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            OnPropertyChanged(_toStringEventArgs);
        }
    }
}
