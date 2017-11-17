using System;
using System.Windows.Input;

namespace NullVoidCreations.WpfHelpers.Base
{
    public abstract class CommandBase : NotificationBase, ICommand
    {
        public event EventHandler CanExecuteChanged;

        bool _isEnabled, _isExecuting, _isSynchonous;
        readonly ViewModelBase _viewModel;

        #region constructor/destructor

        protected CommandBase(bool isEnabled = false)
        {
            IsEnabled = isEnabled;
        }

        #endregion

        #region properties

        public bool IsSynchronous
        {
            get { return _isSynchonous; }
            set { Set("IsSynchronous", ref _isSynchonous, value); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                Set("IsEnabled", ref _isEnabled, value);
                RaiseCanExecuteChanged();
            }
        }

        public bool IsExecuting
        {
            get { return _isExecuting; }
            protected set
            {
                if (value == _isExecuting)
                    return;

                Set("IsExecuting", ref _isExecuting, value);
                RaiseCanExecuteChanged();
            }
        }

        #endregion

        public bool CanExecute(object parameter)
        {
            return IsEnabled && !IsExecuting;
        }

        public abstract void Execute(object parameter);

        void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler == null)
                return;

            handler.Invoke(this, EventArgs.Empty);
        }
    }
}
