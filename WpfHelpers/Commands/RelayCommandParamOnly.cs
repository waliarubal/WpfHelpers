using NullVoidCreations.WpfHelpers.Base;
using System;
using System.ComponentModel;
using System.Windows;

namespace NullVoidCreations.WpfHelpers.Commands
{
    public class RelayCommand<P>: CommandBase
    {
        readonly Action<P> _action;
        BackgroundWorker _worker;

        public RelayCommand(Action<P> action, bool isEnabled = true)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            _action = action;
            IsEnabled = isEnabled;
        }

        public override void Execute(object parameter)
        {
            IsExecuting = true;

            if (IsSynchronous)
            {
                Application.Current.Dispatcher.Invoke(_action, (P)parameter);

                IsExecuting = false;
                return;
            }

            if (_worker == null)
            {
                _worker = new BackgroundWorker();
                _worker.DoWork += Worker_DoWork;
                _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            }
            _worker.RunWorkerAsync(new object[] { _action, parameter });
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsExecuting = false;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var arguments = e.Argument as object[];
            
            var action = arguments[0] as Action<P>;
            action.Invoke((P)arguments[1]);
        }
    }
}
