using NullVoidCreations.WpfHelpers.Base;
using System;
using System.ComponentModel;
using System.Windows;

namespace NullVoidCreations.WpfHelpers.Commands
{
    public class RelayCommand<P, R>: CommandBase 
    {
        readonly Func<P, R> _action;
        readonly Action<R> _callback;
        BackgroundWorker _worker;

        public RelayCommand(Func<P, R> action, Action<R> callback, bool isEnabled = true)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (callback == null)
                throw new ArgumentNullException("callback");

            _action = action;
            _callback = callback;
            IsEnabled = isEnabled;
        }

        public override void Execute(object parameter)
        {
            IsExecuting = true;

            if (IsSynchronous)
            {
                
               var result = (R)Application.Current.Dispatcher.Invoke(_action, (P)parameter);
                _callback.Invoke(result);

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
            var result = (R)e.Result;
            _callback.Invoke(result);

            IsExecuting = false;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var arguments = e.Argument as object[];

            var action = arguments[0] as Func<P, R>;
            e.Result = action.Invoke((P)arguments[1]);
        }
    }
}
